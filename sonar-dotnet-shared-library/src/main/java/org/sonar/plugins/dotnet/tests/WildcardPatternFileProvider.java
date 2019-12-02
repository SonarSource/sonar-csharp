/*
 * SonarSource :: .NET :: Shared library
 * Copyright (C) 2014-2019 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */
package org.sonar.plugins.dotnet.tests;

import java.io.File;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.regex.Pattern;
import java.util.stream.Collectors;
import org.sonar.api.utils.WildcardPattern;
import org.sonar.api.utils.log.Logger;
import org.sonar.api.utils.log.Loggers;

public class WildcardPatternFileProvider {
  private static final Logger LOG = Loggers.get(WildcardPatternFileProvider.class);

  private static final String CURRENT_FOLDER = ".";
  private static final String PARENT_FOLDER = "..";

  private static final String RECURSIVE_PATTERN = "**";
  private static final String ZERO_OR_MORE_PATTERN = "*";
  private static final String ANY_PATTERN = "?";

  private final File baseDir;
  private final String directorySeparator;

  public WildcardPatternFileProvider(File baseDir, String directorySeparator) {
    this.baseDir = baseDir;
    this.directorySeparator = directorySeparator;
  }

  Set<File> listFiles(String pattern) {

    LOG.info("WILDCARD - will list files for " + pattern);

    List<String> elements = Arrays.asList(pattern.split(Pattern.quote(directorySeparator)));

    List<String> elementsTillFirstWildcard = elementsTillFirstWildcard(elements);
    String pathTillFirstWildcardElement = toPath(elementsTillFirstWildcard);
    File fileTillFirstWildcardElement = new File(pathTillFirstWildcardElement);

    File absoluteFileTillFirstWildcardElement = fileTillFirstWildcardElement.isAbsolute() ? fileTillFirstWildcardElement : new File(baseDir, pathTillFirstWildcardElement);

    LOG.info("WILDCARD - absoluteFileTillFirstWildcardElement " + absoluteFileTillFirstWildcardElement.getAbsolutePath());

    List<String> wildcardElements = elements.subList(elementsTillFirstWildcard.size(), elements.size());
    if (wildcardElements.isEmpty()) {
      LOG.info("WILDCARD - Early return because wildcard elements is empty");
      return absoluteFileTillFirstWildcardElement.exists() ? new HashSet<>(Arrays.asList(absoluteFileTillFirstWildcardElement)) : Collections.emptySet();
    }
    checkNoCurrentOrParentFolderAccess(wildcardElements);

    WildcardPattern wildcardPattern = WildcardPattern.create(toPath(wildcardElements), directorySeparator);

    LOG.info("WILDCARD - will add files for wildcardPattern " + wildcardPattern.toString());

    Set<File> result = new HashSet<>();
    for (File file : listFiles(absoluteFileTillFirstWildcardElement)) {
      String relativePath = relativize(absoluteFileTillFirstWildcardElement, file);

      if (wildcardPattern.match(relativePath)) {
        LOG.info("WILDCARD - Adding file to result list " + file.getAbsolutePath());
        result.add(file);
      } else
      {
        LOG.info("WILDCARD - SKIP file " + file.getAbsolutePath() + " because it does not match pattern ");
      }
    }

    LOG.info("WILDCARD - result has " + result.size() + " elements");
    return result;
  }

  private String toPath(List<String> elements) {
    return elements.stream().collect(Collectors.joining(directorySeparator));
  }

  private static List<String> elementsTillFirstWildcard(List<String> elements) {
    List<String> result = new ArrayList<>();
    for (String element : elements) {
      if (containsWildcard(element)) {
        break;
      }
      result.add(element);
    }
    return result;
  }

  private static void checkNoCurrentOrParentFolderAccess(List<String> elements) {
    for (String element : elements) {
      if (isCurrentOrParentFolder(element)) {
        throw new IllegalArgumentException("Cannot contain '" + CURRENT_FOLDER + "' or '" + PARENT_FOLDER + "' after the first wildcard.");
      }
    }
  }

  private static boolean containsWildcard(String element) {
    return RECURSIVE_PATTERN.equals(element) ||
      element.contains(ZERO_OR_MORE_PATTERN) ||
      element.contains(ANY_PATTERN);
  }

  private static boolean isCurrentOrParentFolder(String element) {
    return CURRENT_FOLDER.equals(element) ||
      PARENT_FOLDER.equals(element);
  }

  private static Set<File> listFiles(File dir) {
    Set<File> result = new HashSet<>();
    listFiles(result, dir);
    return result;
  }

  private static void listFiles(Set<File> result, File dir) {
    File[] files = dir.listFiles();
    if (files != null) {
      result.addAll(Arrays.asList(files));

      for (File file : files) {
        if (file.isDirectory()) {
          listFiles(result, file);
        }
      }
    }
  }

  private static String relativize(File parent, File file) {
    return file.getAbsolutePath().substring(parent.getAbsolutePath().length() + 1);
  }

}
