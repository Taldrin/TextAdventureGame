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

log_message "Building individual services"

docker build -t admin-site -f AdminSite/InterfurCreations.AdminSite/Dockerfile .

#if [[ -f "$DOCKER_COMPOSE_FILE" ]]; then
#    log_message "Using Docker Compose"
#    docker-compose -f "$DOCKER_COMPOSE_FILE" down
#    docker-compose -f "$DOCKER_COMPOSE_FILE" up --build -d
#else
#    log_message "Docker Compose file not found: $DOCKER_COMPOSE_FILE. Skipping..."
#fi

log_message "Script completed successfully."
