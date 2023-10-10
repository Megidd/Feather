### Make targets of this file by Windows NMAKE tool.
### NMAKE tool is installed alongside Visual Studio 2022, for example.
### By Windows start menu, launch:
### `Developer Command Prompt for VS 2022`

# To delete outdated files.
clean:
	cd dist
	del Cotton.exe
	del feather-*.yak
	del Feather.rhp
	del System*
	del Microsoft*

# To build strategic binary.
trial:
	cd %userprofile%\go\src\Cotton
	go clean
	go build -tags trial
	copy "Cotton.exe" "%userprofile%\source\repos\Feather\RhinoCommon\Feather\Feather\bin\Release\net48"
	copy "Cotton.exe" "%userprofile%\source\repos\Feather\RhinoCommon\Feather\Feather\bin\Debug\net48"

# To build strategic binary.
commercial:
	cd %userprofile%\go\src\Cotton
	go clean
	go build -tags commercial
	copy "Cotton.exe" "%userprofile%\source\repos\Feather\RhinoCommon\Feather\Feather\bin\Release\net48"
	copy "Cotton.exe" "%userprofile%\source\repos\Feather\RhinoCommon\Feather\Feather\bin\Debug\net48"

# To create the manifest file, if `manifest.yml` doesn't exist already.
manifest:
	"C:\Program Files\Rhino 7\System\Yak.exe" spec

# To distribute plugin package.
pkg:
	cd dist
	copy "%userprofile%\source\repos\Feather\RhinoCommon\Feather\Feather\bin\Release\net48\*" .
	del Feather.pdb
	"C:\Program Files\Rhino 7\System\Yak.exe" build --platform win

# To log in before push to server, if not already logged in:
login:
	"C:\Program Files\Rhino 7\System\Yak.exe" login

# To push plugin package to Rhino3D server.
push:
	cd dist
	echo off > temp.bat
    for /f "delims=" %%i in ('dir /b /od /a-d feather-*-win.yak') do echo "C:\Program Files\Rhino 7\System\Yak.exe" push "%%i" > temp.bat
    call temp.bat
    del temp.bat

# To search for plugin on Rhino3D server.
search:
	"C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease Feather
