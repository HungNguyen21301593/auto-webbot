docker compose down -v
docker-compose up -d --renew-anon-volumes --build --force-recreate

docker image tag autowebbot-api hungnguyen991995/kijiji-helper-api:latest
docker image push hungnguyen991995/kijiji-helper-api:latest

docker image tag autowebbot-index hungnguyen991995/kijiji-helper-index:latest
docker image push hungnguyen991995/kijiji-helper-index:latest
set /p DUMMY=Hit ENTER to continue...
docker compose down -v
set /p DUMMY=Hit ENTER to continue...