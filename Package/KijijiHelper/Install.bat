@REM wsl --install -d Ubuntu
docker-compose down -v
docker-compose pull
docker-compose up -d --renew-anon-volumes --build --force-recreate

timeout 10
start http://localhost:3001/
set /p DUMMY=Hit ENTER to exit...