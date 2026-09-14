const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const { execSync } = require('child_process');

const ROOT_DIR = path.resolve(__dirname, '..');
const PKG_NAME = 'Perkox-Unity-SDK-v2.0.0.unitypackage';
const OUTPUT_PATH = path.join(ROOT_DIR, PKG_NAME);
const TEMP_BUILD_DIR = path.join(ROOT_DIR, '.tmp_package_build');

function getGuid(relPath) {
  return crypto.createHash('md5').update('perkox_sdk_' + relPath).digest('hex');
}

function getMetaContent(filePath, isDir, guid) {
  if (isDir) {
    return `fileFormatVersion: 2\nguid: ${guid}\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`;
  }
  const ext = path.extname(filePath).toLowerCase();
  if (ext === '.cs') {
    return `fileFormatVersion: 2\nguid: ${guid}\nMonoImporter:\n  externalObjects: {}\n  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`;
  }
  if (ext === '.asmdef') {
    return `fileFormatVersion: 2\nguid: ${guid}\nAssemblyDefinitionImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`;
  }
  if (ext === '.aar') {
    return `fileFormatVersion: 2\nguid: ${guid}\nPluginImporter:\n  externalObjects: {}\n  serializedVersion: 2\n  iconMap: {}\n  executionOrder: {}\n  isPreloaded: 0\n  isOverridable: 0\n  isExplicitlyReferenced: 0\n  validateReferences: 1\n  platformData:\n  - first:\n      Android: Android\n    second:\n      enabled: 1\n      settings:\n        CPU: AnyCPU\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`;
  }
  if (ext === '.xcframework') {
    return `fileFormatVersion: 2\nguid: ${guid}\nNativeFormatImporter:\n  externalObjects: {}\n  mainObjectFileID: 0\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`;
  }
  return `fileFormatVersion: 2\nguid: ${guid}\nTextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`;
}

function collectEntries(dir, baseRel = '') {
  let entries = [];
  const files = fs.readdirSync(dir);
  for (const file of files) {
    if (file.startsWith('.') || file.endsWith('.meta') || file === 'node_modules' || file.endsWith('.unitypackage')) continue;
    const fullPath = path.join(dir, file);
    const relPath = baseRel ? `${baseRel}/${file}` : file;
    const stat = fs.statSync(fullPath);
    if (stat.isDirectory()) {
      entries.push({ fullPath, relPath, isDir: true });
      entries = entries.concat(collectEntries(fullPath, relPath));
    } else {
      entries.push({ fullPath, relPath, isDir: false });
    }
  }
  return entries;
}

try {
  console.log('Building Unity Package: ' + PKG_NAME);

  if (fs.existsSync(TEMP_BUILD_DIR)) {
    fs.rmSync(TEMP_BUILD_DIR, { recursive: true, force: true });
  }
  fs.mkdirSync(TEMP_BUILD_DIR, { recursive: true });

  const rootItems = ['Runtime', 'Editor', 'package.json', 'README.md', 'LICENSE', 'CHANGELOG.md'];
  let allEntries = [{ fullPath: '', relPath: 'Perkox', isDir: true }];

  for (const item of rootItems) {
    const itemPath = path.join(ROOT_DIR, item);
    if (!fs.existsSync(itemPath)) continue;
    const isDir = fs.statSync(itemPath).isDirectory();
    allEntries.push({ fullPath: itemPath, relPath: `Perkox/${item}`, isDir });
    if (isDir) {
      allEntries = allEntries.concat(collectEntries(itemPath, `Perkox/${item}`));
    }
  }

  // Also write .meta files in repo for Runtime and Editor
  for (const entry of allEntries) {
    if (entry.relPath === 'Perkox') continue;
    const repoRelPath = entry.relPath.replace(/^Perkox\//, '');
    const metaPathInRepo = path.join(ROOT_DIR, repoRelPath + '.meta');
    const guid = getGuid(entry.relPath);
    const metaContent = getMetaContent(entry.fullPath, entry.isDir, guid);
    fs.writeFileSync(metaPathInRepo, metaContent, 'utf8');
  }

  // Build unitypackage layout in TEMP_BUILD_DIR
  for (const entry of allEntries) {
    const guid = getGuid(entry.relPath);
    const guidDir = path.join(TEMP_BUILD_DIR, guid);
    fs.mkdirSync(guidDir, { recursive: true });

    const unityPath = `Assets/${entry.relPath}`;
    fs.writeFileSync(path.join(guidDir, 'pathname'), unityPath, 'utf8');

    const metaContent = getMetaContent(entry.fullPath, entry.isDir, guid);
    fs.writeFileSync(path.join(guidDir, 'asset.meta'), metaContent, 'utf8');

    if (!entry.isDir && fs.existsSync(entry.fullPath)) {
      fs.copyFileSync(entry.fullPath, path.join(guidDir, 'asset'));
    }
  }

  // Tar and gzip using tar command
  console.log(`Packaging ${allEntries.length} assets into ${PKG_NAME}...`);
  if (fs.existsSync(OUTPUT_PATH)) {
    fs.unlinkSync(OUTPUT_PATH);
  }

  execSync(`tar -czf "${OUTPUT_PATH}" *`, { cwd: TEMP_BUILD_DIR });
  console.log(`Successfully built ${PKG_NAME} (${(fs.statSync(OUTPUT_PATH).size / 1024).toFixed(1)} KB)`);

} finally {
  if (fs.existsSync(TEMP_BUILD_DIR)) {
    fs.rmSync(TEMP_BUILD_DIR, { recursive: true, force: true });
  }
}
