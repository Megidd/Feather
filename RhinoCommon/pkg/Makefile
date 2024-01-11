### Make targets of this file by Windows NMAKE tool.
### NMAKE tool is installed alongside Visual Studio 2022, for example.
### By Windows start menu, launch:
### `Developer Command Prompt for VS 2022`

build:
	cd %userprofile%\repos\Feather\RhinoCommon\Feather
	dotnet build Feather.sln -c Release -f net48 -p:Version=1.6.0.0

# To delete outdated files.
clean:
	cd %userprofile%\repos\Feather\RhinoCommon\pkg\dist
	del Cotton.exe 2> NUL
	del feather-*.yak 2> NUL
	del Feather.rhp 2> NUL
	del System* 2> NUL
	del Microsoft* 2> NUL
	del temp.bat 2> NUL
	del NULL 2> NUL

# To build strategic binary.
trial:
	cd %userprofile%\go\src\Cotton
	go clean
	go build -tags trial
	copy "Cotton.exe" "%userprofile%\repos\Feather\RhinoCommon\Feather\Feather\bin\Release\net48"
	copy "Cotton.exe" "%userprofile%\repos\Feather\RhinoCommon\Feather\Feather\bin\Debug\net48"

# To build strategic binary.
commercial:
	cd %userprofile%\go\src\Cotton
	go clean
	go build -tags commercial
	copy "Cotton.exe" "%userprofile%\repos\Feather\RhinoCommon\Feather\Feather\bin\Release\net48"
	copy "Cotton.exe" "%userprofile%\repos\Feather\RhinoCommon\Feather\Feather\bin\Debug\net48"

# To create the manifest file, if `manifest.yml` doesn't exist already.
# If manifest exists, the version must be incremented before each push to server.
manifest:
	"C:\Program Files\Rhino 7\System\Yak.exe" spec

# Install the binaries inside the distribution folder.
install:
	cd %userprofile%\repos\Feather\RhinoCommon\pkg\dist
	copy "%userprofile%\repos\Feather\RhinoCommon\Feather\Feather\bin\Release\net48\*" .
	del Feather.pdb

# To create plugin package.
pkg:
	cd %userprofile%\repos\Feather\RhinoCommon\pkg\dist
	"C:\Program Files\Rhino 7\System\Yak.exe" build --platform win

# To log in before push to server, if not already logged in:
login:
	"C:\Program Files\Rhino 7\System\Yak.exe" login

# To push plugin package to Rhino3D server.
push:
	cd %userprofile%\repos\Feather\RhinoCommon\pkg\dist
	echo off > temp.bat
    for /f "delims=" %%i in ('dir /b /od /a-d feather-*-win.yak') do echo "C:\Program Files\Rhino 7\System\Yak.exe" push "%%i" > temp.bat
    call temp.bat
    del temp.bat

# To search for plugin on Rhino3D server.
search:
	"C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease Feather

# To create a ZIP file to be uploaded to Patreon shop.
zip:
	cd %userprofile%\repos\Feather\RhinoCommon\pkg
	powershell Compress-Archive -Force dist FEAther.zip

# To create and distribute the trial release as a Rhino3D plugin package.
distribute-trial: clean trial install pkg push search

# To create and distribute the commercial release as a Rhino3D plugin ZIP file.
distribute-commercial: clean commercial install zip
