package com.qk.staticblock;

/**
 * Hello world!
 *
 */
public class App 
{
  public static void main( String[] args )
    {
      System.out.println( "Hello World!" );
      System.out.println( "" );
      Foo.init();
      System.out.println( "" );
      Bar bar = new Bar();
      System.out.println( "" );
    }
}
