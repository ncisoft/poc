package com.nci.utils;

import java.time.LocalDateTime;

public class BuildInfo {

	public static void main(String[] args) {
		String javaVersion = Runtime.version().toString();
		String time = LocalDateTime.now().toString();
		System.out.println("********\nBuild Time: " + time 
				+ "\nJava Version: " + javaVersion + "\n********");
	}

}