$(info $(SHELL))

# Run this target with Git Bash on Windows.
clean:
	DELETE := rm -f
	cd dist && $(DELETE) Cotton.exe
	cd dist && $(DELETE) feather-*.yak
	cd dist && $(DELETE) Feather.rhp
	cd dist && $(DELETE) System*
	cd dist && $(DELETE) Microsoft*

# Run this target with Git Bash on Windows.
trial:
	ifndef HOME
		$(error HOME is not set)
	endif
	cd $(HOME)/go/src/Cotton/ && make clean
	cd $(HOME)/go/src/Cotton/ && make trial
	cd $(HOME)/go/src/Cotton/ && make install

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
