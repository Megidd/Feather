pkg:
	cd dist
	copy ..\Feather\Feather\bin\Release\net48\* .
	"C:\Program Files\Rhino 7\System\Yak.exe" spec
	"C:\Program Files\Rhino 7\System\Yak.exe" build --platform win

push:
	cd dist
	"C:\Program Files\Rhino 7\System\Yak.exe" login
	"C:\Program Files\Rhino 7\System\Yak.exe" push feather-*.yak
	"C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease Feather

clean:
	cd dist
	del Cotton.exe
	del feather-*.yak
	del Feather.rhp
	del System*
	del Microsoft*
