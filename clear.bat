@echo off

for %%d in (
	bin obj .vs _ReSharper.Caches
) do (
	echo Clearing /%%d directories
	for /f %%f in ('dir /s /b /d /a %%d') do (
		@if exist "%%f" (
			echo rd "%%f"
			rd /s /q "%%f"
		)
	)
)

pause