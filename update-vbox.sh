#!/bin/bash

BOXES="cbox lmde atom"
gitdir=$(git rev-parse --show-toplevel 2>/dev/null)
PROJ_DIR=$(realpath --relative-to=$HOME $gitdir)
hostname=$(hostname -s)
case $hostname in
  cbox)
    hostname=cbox
    ;;
  lmde)
    hostname=lmde
    ;;
  debian-atom)
    hostname=atom
    ;;
  *)
    echo "unknown hostname ..$hostname"
    exit 1
    ;;
esac

BRED='\033[1;31m'
NC='\033[0m' # No Color
echo
echo target path is $PROJ_DIR...
for box in  $BOXES;
do
  echo  -e  "${BRED}sync $box with $hostname ...${NC}"
  test ! $box = $hostname && ssh -o ConnectTimeout=2 -x $box "cd $PROJ_DIR && git pull"
  test $box = $hostname && echo "This machine dont need sync"
done
