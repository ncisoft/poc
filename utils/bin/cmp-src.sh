#!/bin/bash

target="$1"
dir="$2"
gitdir=$(git rev-parse --show-toplevel 2>/dev/null)

fg_bright_yellow='[33m[1m'
fg_reset='[0m'

echo_hl()
{
  echo -e ${fg_bright_yellow}$@ ${fg_reset} >&2
}

new_file_list=""

exec_mvn()
{
  true && \
  cp -u src/main/java/com/nci/apps/mock/Test.java src/main/resources/ && \
  mvn --log-file $gitdir/mvn.log process-test-classes && touch $target


}

run()
{
  true &&                                \
    echo_hl "found newer files: [$new_file_list]" && \
    cp -u src/main/java/com/nci/apps/mock/Test.java src/main/resources/ && \
     mvn --log-file $gitdir/mvn.log process-test-classes && touch $target
  exit 0
}

LF='\n'
! test -f $target && run && exit 0
for file in "./pom.xml" $(find "$dir" -name "*.*" ! -name '*.c')
do
  test "$file" -nt "$target" && new_file_list+=" $file"
done

if test -n "$new_file_list"; then
  echo "$new_file_list"
  echo  -e "${fg_bright_yellow}found newer files:\n["
  for new_file in $new_file_list; do
    echo  "  $new_file"
  done
  echo -e  "\n]${fg_reset}"

  cp -u src/main/java/com/nci/apps/mock/Test.java src/main/resources/ && \
    mvn  --log-file /tmp/mvn.log   process-test-classes && touch $target
  exit 0
fi

echo_hl "every thing is clean"
exit 0
