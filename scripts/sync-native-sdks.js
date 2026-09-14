const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

const ANDROID_REPO = 'https://github.com/perkoxofficial/perkox-android-sdk-releases.git';
const IOS_REPO = 'https://github.com/perkoxofficial/perkox-ios-sdk-releases.git';

const ROOT_DIR = path.resolve(__dirname, '..');
const TEMP_DIR = path.join(ROOT_DIR, '.tmp_sdk_sync');

function log(msg) {
  console.log(`[Sync Native SDKs] ${msg}`);
}

function cleanTemp() {
  if (fs.existsSync(TEMP_DIR)) {
    fs.rmSync(TEMP_DIR, { recursive: true, force: true });
  }
}

try {
  cleanTemp();
  fs.mkdirSync(TEMP_DIR, { recursive: true });

  // 1. Sync Android Native SDK
  log('Fetching latest Perkox Android SDK...');
  const localAndroidRelease = path.resolve(ROOT_DIR, '..', 'perkox-android-sdk-releases');
  const destLibs = path.join(ROOT_DIR, 'Runtime', 'Plugins', 'Android', 'libs');
  fs.mkdirSync(destLibs, { recursive: true });
  const destAar = path.join(destLibs, 'perkox-android-sdk-release.aar');

  if (fs.existsSync(path.join(localAndroidRelease, 'perkox-android-sdk-release.aar'))) {
    fs.copyFileSync(path.join(localAndroidRelease, 'perkox-android-sdk-release.aar'), destAar);
    log(`Synced from local repo -> ${destAar}`);
  } else {
    const androidClonePath = path.join(TEMP_DIR, 'android-sdk');
    execSync(`git clone --depth 1 ${ANDROID_REPO} "${androidClonePath}"`, { stdio: 'inherit' });
    const remoteAar = path.join(androidClonePath, 'perkox-android-sdk-release.aar');
    if (fs.existsSync(remoteAar)) {
      fs.copyFileSync(remoteAar, destAar);
      log(`Synced from remote repo -> ${destAar}`);
    }
  }

  // 2. Sync iOS Native SDK
  log('Fetching latest Perkox iOS SDK...');
  const localIosRelease = path.resolve(ROOT_DIR, '..', 'perkox-ios-sdk-releases');
  const destFrameworks = path.join(ROOT_DIR, 'Runtime', 'Plugins', 'iOS', 'Frameworks');
  fs.mkdirSync(destFrameworks, { recursive: true });
  const destXcframework = path.join(destFrameworks, 'PerkoxOfferwall.xcframework');

  if (fs.existsSync(path.join(localIosRelease, 'PerkoxOfferwall.xcframework'))) {
    if (fs.existsSync(destXcframework)) {
      fs.rmSync(destXcframework, { recursive: true, force: true });
    }
    fs.cpSync(path.join(localIosRelease, 'PerkoxOfferwall.xcframework'), destXcframework, { recursive: true });
    log(`Synced from local repo -> ${destXcframework}`);
  } else {
    const iosClonePath = path.join(TEMP_DIR, 'ios-sdk');
    execSync(`git clone --depth 1 ${IOS_REPO} "${iosClonePath}"`, { stdio: 'inherit' });
    const remoteXcframework = path.join(iosClonePath, 'PerkoxOfferwall.xcframework');
    if (fs.existsSync(remoteXcframework)) {
      if (fs.existsSync(destXcframework)) {
        fs.rmSync(destXcframework, { recursive: true, force: true });
      }
      fs.cpSync(remoteXcframework, destXcframework, { recursive: true });
      log(`Synced from remote repo -> ${destXcframework}`);
    }
  }

  log('Successfully synchronized latest Android & iOS native SDKs into Unity Plugins!');
} catch (err) {
  console.error('❌ Failed to sync native SDKs:', err.message);
  process.exit(1);
} finally {
  cleanTemp();
}
