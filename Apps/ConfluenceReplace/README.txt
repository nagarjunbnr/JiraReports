
Options:
	-t	Targets a single space, requires an argument of either the space's key or full name. Can't be mixed with -a
	-a	Targets all spaces in confluence, does not need an argument. Can't be mixed with -t
	-r	The string that is being replaced
	-w	The string that -r is being replaced WITH
	-u	Your Confluence username
	-p	Your confluence password
	-s	The confluence base URL [defaults to https://dev-confluence.affinitiv.com, optional]

Example: replace a string in Space "ENG"
ConfluenceReplace.exe -t "ENG" -r "https://confluence.autoloop.com/x/" -w "https://dev-confluence.affinitiv.com/x/" -u "username" -p "password"


Example: replace a string in all spaces on confluence
ConfluenceReplace.exe -a -r "https://confluence.autoloop.com/x/" -w "https://dev-confluence.affinitiv.com/x/" -u "username" -p "password"
