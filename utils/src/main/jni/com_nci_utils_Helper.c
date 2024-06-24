#include <stdlib.h>
#include "com_nci_utils_Helper.h"

#define TAB2 "  "

static unsigned char hex_tbl[] = {
    '0',
    '1',
    '2',
    '3',
    '4',
    '5',
    '6',
    '7',
    '8',
    '9',
    'a',
    'b',
    'c',
    'd',
    'e',
    'f',
};
static void dump_memoey_block(unsigned char *cp)
{
  unsigned char * cp_head = cp;
  unsigned char * cp_tail = cp_head + 8;

  if (!cp)
    return;
  printf("%s%p:  ", TAB2, cp_head);
  for (cp = cp_head; cp < cp_tail; cp++)
    {
      unsigned int ch=*cp;
      unsigned int hi_byte = ch >> 4;
      unsigned int low_byte = ch & 0xf;
      printf("%c%c ", hex_tbl[hi_byte], hex_tbl[low_byte]);
    }
  printf("\n");
}

JNIEXPORT void JNICALL Java_com_nci_utils_Helper_getAge
  (JNIEnv *env, jclass thisClass, jobject o)
{
  //jlong address;
  printf("------begin %s\n", __func__);
  printf("\n%s%p:", TAB2, o);
    {
      unsigned char *cp = (unsigned char *)o;
      unsigned int age = *cp;
      age &= 0x78;
      printf("0x%x %d\n", age, age);
    }
    dump_memoey_block(NULL);
  printf("\n");
  printf("------end %s\n", __func__);
  fflush(stdout);

}

JNIEXPORT void JNICALL Java_com_nci_utils_Helper_getAgeByAddress
  (JNIEnv *env, jclass thisClass, jlong address)
  {
      unsigned char *cp = (unsigned char *)address;
      unsigned int age = *cp;
      age &= 0x78;
      printf("%s%p:\n", TAB2, cp);
      printf("0x%x %d\n", age, age);
      dump_memoey_block(cp);

  }