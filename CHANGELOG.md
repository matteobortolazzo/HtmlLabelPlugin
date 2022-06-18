# 5.1.0

## Update
* Update to `Xamarin.Forms` v5

# 5.0.2

## Bug Fix
* **iOS**: Fix freeze on iOS. [#137](https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/137)

# 5.0.0

## Breaking Changes
* **iOS**: The renderer has been implement with `UITextView`.

## Bug Fix
* **iOS**: Custom fonts fix.
* **iOS**: Link tap with multiple lines fix.
* **Android**: Fix bullet point indent.
* **Android**: Fix `tel://` links with whitespaces.
* **All**: `mailto` with no subject or body fix.

# 4.1.3

## Bug Fix
* **iOS**: Fix multi-line labels with links.

# 4.1.2

## Bug Fix
* **All**: *mailto*, *tel*, *sms*, *geo* handled properly. Fixes [#92](https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/82) [#93](https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/82)

# 4.1.1

## Bug Fix
* **iOS**: Default font size (16) not working. [#86](https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/86)
* **iOS**: Unwanted new line. [#87](https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/87)
* **iOS**: Custom fonts were not working. [#88](https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/88)
* **iOS**: Support for LineHeight. [#90](https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/90)
* **Android**: Unwanted new line. [#87](https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/87)

# 4.1.0

## Features
* **Android:** Support for Android's Legacy Mode rendering. [#83](https://github.com/matteobortolazzo/HtmlLabelPlugin/pull/83)
* **Android:** Support for images. [#49](https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/49)

# 4.0.1

## Bug Fix
* **iOS:** Fix system font. [#81](https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/81)

# 4.0.0

## Features
* **Links:** New *Underline* property (bool).
* **Links:** New *LinkColor* property.
* **Links:** New *BrowserLaunchOptions* property.

## Bug Fix
* **General:** RTL support.
* **iOS:** Link click detection rewritten.
* **Android:** Lists not displayed correctly ([#76](https://github.com/matteobortolazzo/HtmlLabelPlugin/pull/76)
). Thanks to [AlexSchuetz](https://github.com/AlexSchuetz).