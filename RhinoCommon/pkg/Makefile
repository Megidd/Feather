# Make targets of this file by Windows NMAKE tool.
# NMAKE tool is installed alongside Visual Studio 2022, for example.
# By Windows start menu, launch:
# `Developer Command Prompt for VS 2022`

clean:
	cd dist
	-rem Deleting outdated files...
	-@del Cotton.exe
	-@del feather-*.yak
	-@del Feather.rhp
	-@del System*
	-@del Microsoft*


trial:
	cd %userprofile%\go\src\Cotton
	make clean
	make trial
	make install

commercial:
	cd $(HOME)/go/src/Cotton/ && make clean
	cd $(HOME)/go/src/Cotton/ && make commercial
	cd $(HOME)/go/src/Cotton/ && make install

pkg:
	cd %userprofile%\source\repos\Feather\RhinoCommon\pkg\dist\
	copy %userprofile%\source\repos\Feather\RhinoCommon\Feather\Feather\bin\Release\net48\* .
	del Feather.pdb
	REM Skip if manifest.yml exists:
	REM "C:\Program Files\Rhino 7\System\Yak.exe" spec
	"C:\Program Files\Rhino 7\System\Yak.exe" build --platform win

push:
	cd %userprofile%\source\repos\Feather\RhinoCommon\pkg\dist\
	REM Skip if already logged in:
	REM "C:\Program Files\Rhino 7\System\Yak.exe" login
	"C:\Program Files\Rhino 7\System\Yak.exe" push feather-*.yak
	"C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease Feather
