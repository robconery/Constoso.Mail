run:
	. ./start.sh

test: db
	dotnet test

db:
	psql contoso < ./Data/db.sql --quiet

mailpit:
	docker run -d \
	--restart unless-stopped \
	--name=mailpit \
	-p 8025:8025 \
	-p 1025:1025 \
	axllent/mailpit


.phony: test db seed change_1 mailhog

#pg_dump -d contoso --no-owner --no-privileges --no-password --table "mail.contacts" --inserts -f ./dump.sql
#have to find/replace using ^\d*.\t