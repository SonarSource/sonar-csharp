/*
 * SonarSource :: C# :: ITs :: Plugin
 * Copyright (C) 2011-2019 SonarSource SA
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
package com.sonar.it.csharp;

import com.sonar.it.shared.TestUtils;
import com.sonar.orchestrator.Orchestrator;
import com.sonar.orchestrator.http.HttpMethod;
import java.io.IOException;
import java.nio.file.Path;
import org.junit.Before;
import org.junit.ClassRule;
import org.junit.Rule;
import org.junit.Test;
import org.junit.rules.TemporaryFolder;

import static com.sonar.it.csharp.Tests.getMeasureAsInt;
import static org.assertj.core.api.Assertions.assertThat;

public class AutoGeneratedTest {
    @ClassRule
    public static final Orchestrator orchestrator = Tests.ORCHESTRATOR;

    @Rule
    public TemporaryFolder temp = new TemporaryFolder();

    @Before
    public void init() {
      orchestrator.resetData();
    }

    @Test
    public void autogenerated_code_reports_no_bugs_when_generated_not_analyzed() throws Exception {
      final String PROJECT_NAME = "AutoGeneratedFiles";
      analyzeCoverageTestProject(PROJECT_NAME, false);

      assertThat(getMeasureAsInt(PROJECT_NAME, "files")).isNull(); // no files are reported to SQ
      assertThat(getMeasureAsInt(PROJECT_NAME, "new_bugs")).isNull();
    }

    @Test
    public void autogenerated_code_reports_bugs_when_generated_analyzed() throws Exception {
      final String PROJECT_NAME = "AutoGeneratedFiles";
      analyzeCoverageTestProject(PROJECT_NAME, true);

      assertThat(getMeasureAsInt(PROJECT_NAME, "ncloc")).isEqualTo(261);
      assertThat(getMeasureAsInt(PROJECT_NAME, "bugs")).isEqualTo(18);
    }

    @Test
    public void non_autogenerated_code_reports_bugs_when_generated_not_analyzed() throws Exception {
      final String PROJECT_NAME = "NotAutoGeneratedFiles";
      analyzeCoverageTestProject(PROJECT_NAME, false);

      assertThat(getMeasureAsInt(PROJECT_NAME, "ncloc")).isEqualTo(29);
      assertThat(getMeasureAsInt(PROJECT_NAME, "bugs")).isEqualTo(1);
    }

    @Test
    public void non_autogenerated_code_reports_bugs() throws Exception {
      final String PROJECT_NAME = "NotAutoGeneratedFiles";
      analyzeCoverageTestProject(PROJECT_NAME, true);

      assertThat(getMeasureAsInt(PROJECT_NAME, "ncloc")).isEqualTo(29);
      assertThat(getMeasureAsInt(PROJECT_NAME, "bugs")).isEqualTo(1);
    }

    private void analyzeCoverageTestProject(String projectName, boolean analyzeGenerated) throws IOException {
      orchestrator.getServer()
        .newHttpCall("/api/settings/set")
        .setAdminCredentials()
        .setMethod(HttpMethod.POST)
        .setParam("key", "sonar.cs.analyzeGeneratedCode")
        .setParam("value", "" + analyzeGenerated)
        .execute();

      Path projectDir = Tests.projectDir(temp, projectName);
      orchestrator.executeBuild(TestUtils.newScanner(projectDir)
        .addArgument("begin")
        .setProjectKey(projectName)
        .setProjectName(projectName)
        .setProjectVersion("1.0"));

      TestUtils.runMSBuild(orchestrator, projectDir, "/t:Rebuild");

      orchestrator.executeBuild(TestUtils.newScanner(projectDir)
        .addArgument("end"));
    }
}
