/*
 * SonarQube .NET Tests Library
 * Copyright (C) 2014-2018 SonarSource SA
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

import org.junit.Rule;
import org.junit.Test;
import org.junit.rules.ExpectedException;

import java.io.File;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.mock;

public class VisualStudioTestResultsFileParserTest {

  @Rule
  public ExpectedException thrown = ExpectedException.none();

  @Test
  public void no_counters() {
    thrown.expect(IllegalArgumentException.class);
    thrown.expectMessage("The mandatory <Counters> tag is missing in ");
    thrown.expectMessage(new File("src/test/resources/visualstudio_test_results/no_counters.trx").getAbsolutePath());
    new VisualStudioTestResultsFileParser().accept(new File("src/test/resources/visualstudio_test_results/no_counters.trx"), mock(UnitTestResults.class));
  }

  @Test
  public void wrong_passed_number() {
    thrown.expect(ParseErrorException.class);
    thrown.expectMessage("Expected an integer instead of \"foo\" for the attribute \"passed\" in ");
    thrown.expectMessage(new File("src/test/resources/visualstudio_test_results/wrong_passed_number.trx").getAbsolutePath());
    new VisualStudioTestResultsFileParser().accept(new File("src/test/resources/visualstudio_test_results/wrong_passed_number.trx"), mock(UnitTestResults.class));
  }

  @Test
  public void valid() throws Exception {
    UnitTestResults results = new UnitTestResults();
    new VisualStudioTestResultsFileParser().accept(new File("src/test/resources/visualstudio_test_results/valid.trx"), results);

    assertThat(results.failures()).isEqualTo(14);
    assertThat(results.errors()).isEqualTo(3);
    assertThat(results.tests()).isEqualTo(43);
    assertThat(results.passedPercentage()).isEqualTo(14 * 100.0 / 43);
    assertThat(results.skipped()).isEqualTo(12); // 43 - 31
    assertThat(results.executionTime()).isEqualTo(816l);
  }

  @Test
  public void valid_missing_attributes() throws Exception {
    UnitTestResults results = new UnitTestResults();
    new VisualStudioTestResultsFileParser().accept(new File("src/test/resources/visualstudio_test_results/valid_missing_attributes.trx"), results);

    assertThat(results.tests()).isEqualTo(3);
    assertThat(results.passedPercentage()).isEqualTo(3 * 100.0 / 3);
    assertThat(results.skipped()).isEqualTo(0);
    assertThat(results.failures()).isEqualTo(0);
    assertThat(results.errors()).isEqualTo(0);
  }

}
