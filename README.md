[![License](http://img.shields.io/npm/l/xmlbuilder.svg?style=flat-square)](http://opensource.org/licenses/MIT) [![Nuget](https://img.shields.io/nuget/v/ExifLibNet.Updated.svg?style=flat-square)](https://www.nuget.org/packages/ExifLibNet.Updated)

This is an updated fork of ExifLibrary, a .Net Standard and .NET library for editing Exif metadata contained in image files.  
It mainly removes the dependency on `netstandard1.3` to avoid pulling in loads of dependencies and has merged a PR for better DateTime parsing.

# Installation

If you are using [NuGet](https://nuget.org/) you can install it with:

`PM> Install-Package ExifLibNet.Updated`

# Quick Start

To read an image file and extract metadata:

```cs
var file = ImageFile.FromFile("path_to_image");

// the type of the ISO speed rating tag value is unsigned short
// see documentation for tag data types
var isoTag = file.Properties.Get<ExifUShort>(ExifTag.ISOSpeedRatings);

// the flash tag's value is an enum
var flashTag = file.Properties.Get<ExifEnumProperty<Flash>>(ExifTag.Flash);

// GPS latitude is a custom type with three rational values
// representing degrees/minutes/seconds of the latitude 
var latTag = file.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLatitude);
```

To add metadata:

```cs
var file = ImageFile.FromFile("path_to_image");
// note the explicit cast to ushort
file.Properties.Set(ExifTag.ISOSpeedRatings, <ushort>200);

//for pngs
var pngText = new PngText(ExifTag.PNGTagName, ...)
file.Properties.Set(pngText)
```

To save the image with metadata:

```cs
file.Save("path_to_image");
```

# Documentation

Please visit: http://oozcitak.github.io/exiflibrary/