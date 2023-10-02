pkg:
	cd dist && copy ..\..\Feather\Feather\bin\Release\net48\* .
	cd dist && del Feather.pdb
	### Existing manifest.yml. Skipping:
	# cd dist && "C:\Program Files\Rhino 7\System\Yak.exe" spec
	cd dist && "C:\Program Files\Rhino 7\System\Yak.exe" build --platform win

push:
	cd dist && "C:\Program Files\Rhino 7\System\Yak.exe" login
	cd dist && "C:\Program Files\Rhino 7\System\Yak.exe" push feather-*.yak
	cd dist && "C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease Feather

clean:
	cd dist && del Cotton.exe
	cd dist && del feather-*.yak
	cd dist && del Feather.rhp
	cd dist && del System*
	cd dist && del Microsoft*
