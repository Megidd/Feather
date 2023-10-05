REM clean:
	cd %userprofile%\source\repos\Feather\RhinoCommon\pkg\dist\
	del Cotton.exe
	del feather-*.yak
	del Feather.rhp
	del System*
	del Microsoft*

REM build strategic code:
	cd %userprofile%\go\src\Cotton\
	go build -tags trial
	REM go build -tags commercial
	copy /y Cotton.exe %userprofile%\source\repos\Feather\RhinoCommon\Feather\Feather\bin\Release\net48\

REM pkg:
	cd %userprofile%\source\repos\Feather\RhinoCommon\pkg\dist\
	copy %userprofile%\source\repos\Feather\RhinoCommon\Feather\Feather\bin\Release\net48\* .
	del Feather.pdb
	REM Skip if manifest.yml exists:
	REM "C:\Program Files\Rhino 7\System\Yak.exe" spec
	"C:\Program Files\Rhino 7\System\Yak.exe" build --platform win

REM push:
	cd %userprofile%\source\repos\Feather\RhinoCommon\pkg\dist\
	REM Skip if already logged in:
	REM "C:\Program Files\Rhino 7\System\Yak.exe" login
	"C:\Program Files\Rhino 7\System\Yak.exe" push feather-*.yak
	"C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease Feather
