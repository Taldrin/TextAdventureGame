#!/bin/bash

set -e

REPO_DIR="."
REPO_BRANCH="master"
DOCKER_COMPOSE_FILE="docker-compose.yml"

function log_message {
    echo "[INFO] $1"
}

log_message "Navigating to repository directory: $REPO_DIR"
cd "$REPO_DIR" || { echo "[ERROR] Directory not found: $REPO_DIR"; exit 1; }

log_message "Pulling the latest changes from branch: $REPO_BRANCH"
git checkout "$REPO_BRANCH"
git pull origin "$REPO_BRANCH"

log_message "Building and running applications with Docker"


log_message "Building individual services"
cd ./AdventureGameCore
docker build -t furventure-bot-core -f InterfurCreations.AdventureGames.WorkerService/Dockerfile .

cd ../
docker build -t furventure-server -f PublicSite/PublicSite/Server/Dockerfile .
docker build -t furventure-publicsite -f PublicSite/PublicSite/Client/Dockerfile .

#if [[ -f "$DOCKER_COMPOSE_FILE" ]]; then
#    log_message "Using Docker Compose"
#    docker-compose -f "$DOCKER_COMPOSE_FILE" down
#    docker-compose -f "$DOCKER_COMPOSE_FILE" up --build -d
#else
#    log_message "Docker Compose file not found: $DOCKER_COMPOSE_FILE. Skipping..."
#fi

log_message "Script completed successfully."
