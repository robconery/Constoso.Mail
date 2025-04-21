run:
	. ./start.sh

test: db
	dotnet test

db:
	sqlite3 < ./Data/db_sqlite.sql


.phony: test db seed change_1 mailhog

#pg_dump -d contoso --no-owner --no-privileges --no-password --table "mail.contacts" --inserts -f ./dump.sql
#have to find/replace using ^\d*.\t