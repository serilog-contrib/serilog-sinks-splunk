build:
	sh build.sh

test: build
	echo "TODO"

run-samples:
	docker-compose up --build

kill-samples:
	docker-compose down