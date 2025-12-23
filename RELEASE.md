# How to Release SmallSchedulingApp

GitHub Actions is now configured to automatically build and publish releases!

## Creating a New Release

### Step 1: Create a Version Tag

```bash
# Make sure you're on the main branch with all changes committed
git checkout main
git pull

# Create a version tag (e.g., v1.0.0, v1.0.1, v2.0.0)
git tag v1.0.0

# Push the tag to GitHub
git push origin v1.0.0
```

### Step 2: Wait for Build

1. Go to your repository on GitHub
2. Click on the **Actions** tab
3. You'll see the "Build and Release" workflow running
4. Wait 5-10 minutes for it to complete

### Step 3: Check the Release

1. Go to **Releases** in your repository
2. You'll see a new release with your version tag
3. The release will include:
   - `SmallSchedulingApp_x.x.x_x64.msix` - The installer
   - `SmallSchedulingApp.cer` - Security certificate
   - `Install.ps1` - Installation script
   - `INSTALLATION.md` - User instructions

## Manual Trigger (Optional)

You can also manually trigger a build without creating a tag:

1. Go to the **Actions** tab
2. Select "Build and Release" workflow
3. Click **Run workflow**
4. The artifacts will be uploaded but won't create a GitHub Release

## Version Naming Convention

Use semantic versioning:
- `v1.0.0` - Major release
- `v1.1.0` - Minor feature additions
- `v1.0.1` - Bug fixes and patches

## What Users Will Download

Users will get:
1. An MSIX installer package
2. A certificate file (for first-time installation)
3. An automated installation script (`Install.ps1`)
4. Complete installation instructions

Most users will just need to:
1. Download all files
2. Run `Install.ps1` as Administrator
3. Done!

## Troubleshooting

### Build fails:
- Check the Actions logs for errors
- Make sure all code changes are committed
- Verify the project builds locally first

### Can't find the release:
- Check that you pushed the tag with `git push origin v1.0.0`
- Verify the workflow completed successfully in Actions

## Testing Before Release

Before creating an official release, you can test the build:

```bash
# Create a test tag
git tag v0.0.1-test
git push origin v0.0.1-test

# Check the Actions tab to see if build succeeds
# If it works, delete the test tag and release
git tag -d v0.0.1-test
git push origin :refs/tags/v0.0.1-test
```

## First Release Example

```bash
# Commit all your changes
git add .
git commit -m "Prepare for v1.0.0 release"
git push

# Create and push the release tag
git tag v1.0.0
git push origin v1.0.0

# GitHub Actions will automatically build and create the release!
```
