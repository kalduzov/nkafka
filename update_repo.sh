#!/bin/sh

git submodule init
git submodule update

ln -s kafka/clients/src/main/resources/common/message resources/message