#!/bin/sh
(
set -eu
# Official entrypoint sources this file on first initialization only. Never echo passwords.
psql --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" --set=ON_ERROR_STOP=1 \
    --set=app_password="$(cat /run/secrets/db_app_password)" <<'SQL'
CREATE ROLE mongscape LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE PASSWORD :'app_password';
ALTER DATABASE mongscape OWNER TO mongscape;
REVOKE ALL ON DATABASE mongscape FROM PUBLIC;
GRANT CONNECT, TEMPORARY ON DATABASE mongscape TO mongscape;
REVOKE ALL ON SCHEMA public FROM PUBLIC;
GRANT USAGE, CREATE ON SCHEMA public TO mongscape;
SQL
)
