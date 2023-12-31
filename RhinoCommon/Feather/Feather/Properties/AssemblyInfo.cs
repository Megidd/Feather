﻿using Rhino.PlugIns;
using System.Runtime.InteropServices;

// Plug-in Description Attributes - all of these are optional.
// These will show in Rhino's option dialog, in the tab Plug-ins.
[assembly: PlugInDescription(DescriptionType.Address, "Yonge St, Richmond Hill, ON")]
[assembly: PlugInDescription(DescriptionType.Country, "Canada")]
[assembly: PlugInDescription(DescriptionType.Email, "megiddgit@gmail.com")]
[assembly: PlugInDescription(DescriptionType.Phone, "")]
[assembly: PlugInDescription(DescriptionType.Fax, "")]
[assembly: PlugInDescription(DescriptionType.Organization, "www.nano3dtech.com")]
[assembly: PlugInDescription(DescriptionType.UpdateUrl, "")]
[assembly: PlugInDescription(DescriptionType.WebSite, "https://github.com/Megidd/Feather")]

// Icons should be Windows .ico files and contain 32-bit images in the following sizes: 16, 24, 32, 48, and 256.
[assembly: PlugInDescription(DescriptionType.Icon, "Feather.EmbeddedResources.plugin-utility.ico")]

// The following GUID is for the ID of the typelib if this project is exposed to COM
// This will also be the Guid of the Rhino plug-in
[assembly: Guid("50bd62c6-f7bc-486d-8e3f-fa4002487654")]
