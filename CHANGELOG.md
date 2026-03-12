# Changelog

All notable changes to this project will be documented in this file.

---

## [1.1.0] - 2025-03-12

### Added
- Single instance application support
  - Prevent multiple instances from running simultaneously
  - Automatically activate existing window when attempting to launch again
  - Works correctly even when minimized to system tray
- Handwritten signature page
  - Convert signature to Base64 string
  - Adjustable pen size with smooth strokes
  - Save signature as image file (PNG, JPEG, BMP, GIF)

### Changed
- Removed balloon tip notification when minimizing to tray
- Signature page UI improvements
  - Buttons changed to icon style
  - Pen size control repositioned to bottom toolbar
  - Default pen size changed to 7
  - Removed image preview, showing Base64 string directly
- Home page cards repositioned for better visual centering

### Improved
- Cleaned up unused using statements in MainWindow.xaml.cs
- Better stroke smoothing with FitToCurve enabled

---

## [1.0.2] - 2025-03-11

### Added
- Single instance application support
  - Prevent multiple instances from running simultaneously
  - Automatically activate existing window when attempting to launch again
  - Works correctly even when minimized to system tray

### Fixed
- Balloon tip icon not displaying when minimized to tray
  - Changed from system icon to application icon for better visibility

### Improved
- Cleaned up unused using statements in MainWindow.xaml.cs

---

## [1.0.1] - 2025-03-10

### Added
- Image preview and save functionality for Base64 decoder
  - Click image to preview in full window
  - Save image to local file (PNG, JPEG, BMP, GIF)
- Close confirmation dialog with minimize to tray option
  - Remember user's choice during session
  - Ask again after app restart
- Multi-architecture build support (x86, x64, arm64)
- MSI installer with Inno Setup
- Code signing with self-signed certificate
- Application icon for exe and installer

### Changed
- Renamed application to "开发者工具" (Chinese) / "DevTools" (English)
- Updated all namespaces from development_kits to DevTools
- Replaced conversion button icons with more appropriate ones
- Removed home page title and centered cards vertically
- Translated all Chinese comments to English
- Localized all message box prompts using i18n

### Fixed
- JSON formatter double-click selection now excludes quotes for string values
- JSON formatter arrow direction syncs with expand/collapse state
- JSON formatter expand/collapse buttons work recursively
- MD5 copy button error when clicking quickly
- Window title now displays correct localized name
- Missing application icon on exe files
- Windows 11 compatibility warning for installer
- Preview window button text visibility issues
- Close dialog button styling and text color

### Improved
- Better button styling across all dialogs
- Improved clipboard operations with retry logic
- Better error handling for image operations

---

## [1.0.0] - 2025-03-08

### Added
- MD5 hash calculation tool (32-bit and 16-bit, uppercase and lowercase)
- Barcode generator (CODE 128 format)
- QR Code generator
- Base64 to Image decoder
- Image to Base64 encoder
- JSON formatter with expand/collapse functionality
- Multi-language support (Chinese and English)
- Dark theme UI

### Features
- Double-click to select JSON property values
- Copy to clipboard with feedback
- Save generated barcodes and QR codes
- Toggle visibility for generated images

---
