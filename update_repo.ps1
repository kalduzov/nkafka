git submodule init
git submodule update
New-Item -ItemType Junction -Path "resources/message" -Target "kafka/clients/src/main/resources/common/message"