#!/bin/bash

set -e

echo "🚀 Starting FileSyncHub Remotely Environment"
echo ""

export $(cat .env | xargs) && rails c
AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY