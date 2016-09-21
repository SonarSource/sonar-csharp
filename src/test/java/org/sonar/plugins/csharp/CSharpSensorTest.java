/*
 * SonarQube C# Plugin
 * Copyright (C) 2014-2016 SonarSource SA
 * mailto:contact AT sonarsource DOT com
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
package org.sonar.plugins.csharp;

import com.google.common.base.Charsets;
import com.google.common.collect.ImmutableList;
import com.google.common.collect.ImmutableSet;
import com.google.common.collect.Lists;
import com.google.common.io.Files;
import java.io.File;
import java.util.List;
import org.apache.commons.lang.SystemUtils;
import org.junit.Assume;
import org.junit.Before;
import org.junit.Test;
import org.junit.rules.ExpectedException;
import org.mockito.ArgumentCaptor;
import org.mockito.Mockito;
import org.sonar.api.batch.SensorContext;
import org.sonar.api.batch.fs.InputFile;
import org.sonar.api.batch.fs.InputFile.Type;
import org.sonar.api.batch.fs.internal.DefaultFileSystem;
import org.sonar.api.batch.fs.internal.DefaultInputFile;
import org.sonar.api.component.ResourcePerspectives;
import org.sonar.api.config.Settings;
import org.sonar.api.issue.Issuable;
import org.sonar.api.issue.Issuable.IssueBuilder;
import org.sonar.api.issue.Issue;
import org.sonar.api.issue.NoSonarFilter;
import org.sonar.api.measures.CoreMetrics;
import org.sonar.api.measures.FileLinesContext;
import org.sonar.api.measures.FileLinesContextFactory;
import org.sonar.api.measures.Measure;
import org.sonar.api.profiles.RulesProfile;
import org.sonar.api.resources.Project;
import org.sonar.api.resources.Resource;
import org.sonar.api.rule.RuleKey;
import org.sonar.api.rules.ActiveRule;
import org.sonar.api.rules.ActiveRuleParam;
import org.sonar.api.rules.Rule;
import org.sonar.api.rules.RuleParam;

import static org.fest.assertions.Assertions.assertThat;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

public class CSharpSensorTest {

  @org.junit.Rule
  public ExpectedException thrown = ExpectedException.none();

  private SensorContext context;
  private Project project;
  private CSharpSensor sensor;
  private Settings settings;
  private DefaultInputFile inputFile;
  private DefaultFileSystem fs;
  private FileLinesContext fileLinesContext;
  private FileLinesContextFactory fileLinesContextFactory;
  private RuleRunnerExtractor extractor;
  private NoSonarFilter noSonarFilter;
  private ResourcePerspectives perspectives;
  private Issuable issuable;
  private Issuable projectIssuable;
  private IssueBuilder issueBuilder1;
  private IssueBuilder issueBuilder2;
  private IssueBuilder issueBuilder3;
  private IssueBuilder issueBuilder4;
  private IssueBuilder issueBuilder5;
  private Issue issue1;
  private Issue issue2;
  private Issue issue3;
  private Issue issue4;
  private Issue issue5;
  private ActiveRule parametersActiveRule;
  private ActiveRule customRoslynActiveRule;
  private List<ActiveRule> allEnabledRules = Lists.newArrayList();

  @Test
  public void shouldExecuteOnProject() {
    Assume.assumeTrue(SystemUtils.IS_OS_WINDOWS);

    DefaultFileSystem fs = new DefaultFileSystem();

    CSharpSensor sensor = new CSharpSensor(
      mock(Settings.class), mock(RuleRunnerExtractor.class),
      fs,
      mock(FileLinesContextFactory.class), mock(NoSonarFilter.class), mock(RulesProfile.class), mock(ResourcePerspectives.class));

    assertThat(sensor.shouldExecuteOnProject(mock(Project.class))).isFalse();

    fs.add(new DefaultInputFile("foo").setAbsolutePath("foo").setLanguage("java"));
    assertThat(sensor.shouldExecuteOnProject(mock(Project.class))).isFalse();

    fs.add(new DefaultInputFile("bar").setAbsolutePath("bar").setLanguage("cs").setType(Type.TEST));
    assertThat(sensor.shouldExecuteOnProject(mock(Project.class))).isFalse();

    fs.add(new DefaultInputFile("baz").setAbsolutePath("baz").setLanguage("cs"));
    assertThat(sensor.shouldExecuteOnProject(mock(Project.class))).isTrue();
  }

  @Before
  public void init() {
    fs = new DefaultFileSystem();
    fs.setWorkDir(new File("src/test/resources/CSharpSensorTest"));

    inputFile = new DefaultInputFile("Foo&Bar.cs").setAbsolutePath("Foo&Bar.cs").setLanguage("cs");
    fs.add(inputFile);

    fileLinesContext = mock(FileLinesContext.class);
    fileLinesContextFactory = mock(FileLinesContextFactory.class);
    when(fileLinesContextFactory.createFor(inputFile)).thenReturn(fileLinesContext);

    extractor = mock(RuleRunnerExtractor.class);
    when(extractor.executableFile()).thenReturn(new File("src/test/resources/CSharpSensorTest/fake.bat"));

    noSonarFilter = mock(NoSonarFilter.class);
    perspectives = mock(ResourcePerspectives.class);
    issuable = mock(Issuable.class);
    issueBuilder1 = mock(IssueBuilder.class);
    issueBuilder2 = mock(IssueBuilder.class);
    issueBuilder3 = mock(IssueBuilder.class);
    issueBuilder4 = mock(IssueBuilder.class);
    issueBuilder5 = mock(IssueBuilder.class);
    when(issuable.newIssueBuilder()).thenReturn(issueBuilder1, issueBuilder2, issueBuilder3, issueBuilder4);
    issue1 = mock(Issue.class);
    when(issueBuilder1.build()).thenReturn(issue1);
    issue2 = mock(Issue.class);
    when(issueBuilder2.build()).thenReturn(issue2);
    issue3 = mock(Issue.class);
    when(issueBuilder3.build()).thenReturn(issue3);
    issue4 = mock(Issue.class);
    when(issueBuilder4.build()).thenReturn(issue4);
    when(perspectives.as(Mockito.eq(Issuable.class), Mockito.any(InputFile.class))).thenReturn(issuable);
    issue5 = mock(Issue.class);
    when(issueBuilder5.build()).thenReturn(issue5);
    projectIssuable = mock(Issuable.class);
    when(projectIssuable.newIssueBuilder()).thenReturn(issueBuilder5);
    when(perspectives.as(Mockito.eq(Issuable.class), Mockito.any(Resource.class))).thenReturn(projectIssuable);

    ActiveRule templateActiveRule = mock(ActiveRule.class);
    when(templateActiveRule.getRuleKey()).thenReturn("[template_key\"'<>&]");
    when(templateActiveRule.getRepositoryKey()).thenReturn("csharpsquid");
    Rule templateRule = mock(Rule.class);
    Rule baseTemplateRule = mock(Rule.class);
    when(baseTemplateRule.getKey()).thenReturn("[base_key]");
    when(baseTemplateRule.getRepositoryKey()).thenReturn("csharpsquid");
    when(templateRule.getTemplate()).thenReturn(baseTemplateRule);
    when(templateActiveRule.getRule()).thenReturn(templateRule);

    parametersActiveRule = mock(ActiveRule.class);
    when(parametersActiveRule.getRuleKey()).thenReturn("[parameters_key]");
    when(parametersActiveRule.getRepositoryKey()).thenReturn("csharpsquid");
    ActiveRuleParam param1 = mock(ActiveRuleParam.class);
    when(param1.getKey()).thenReturn("[param1_key]");
    when(param1.getValue()).thenReturn("[param1_value]");
    when(parametersActiveRule.getActiveRuleParams()).thenReturn(ImmutableList.of(param1));
    Rule parametersRule = mock(Rule.class);
    RuleParam param1Default = mock(org.sonar.api.rules.RuleParam.class);
    when(param1Default.getKey()).thenReturn("[param1_key]");
    when(param1Default.getDefaultValue()).thenReturn("[param1_default_value]");
    RuleParam param2Default = mock(org.sonar.api.rules.RuleParam.class);
    when(param2Default.getKey()).thenReturn("[param2_default_key]");
    when(param2Default.getDefaultValue()).thenReturn("[param2_default_value]");
    when(parametersRule.getParams()).thenReturn(ImmutableList.of(param1Default, param2Default));
    when(parametersActiveRule.getRule()).thenReturn(parametersRule);

    customRoslynActiveRule = mock(ActiveRule.class);
    when(customRoslynActiveRule.getRuleKey()).thenReturn("custom-roslyn");
    when(customRoslynActiveRule.getRepositoryKey()).thenReturn("roslyn.foo");

    RulesProfile rulesProfile = mock(RulesProfile.class);
    when(rulesProfile.getActiveRulesByRepository("csharpsquid")).thenReturn(ImmutableList.of(templateActiveRule, parametersActiveRule));
    allEnabledRules.add(templateActiveRule);
    allEnabledRules.add(parametersActiveRule);
    when(rulesProfile.getActiveRules()).thenReturn(allEnabledRules);

    settings = mock(Settings.class);
    sensor = new CSharpSensor(
      settings, extractor,
      fs,
      fileLinesContextFactory, noSonarFilter, rulesProfile, perspectives);

    project = mock(Project.class);
    context = mock(SensorContext.class);
  }

  private void enableCustomRoslynRules() {
    allEnabledRules.add(customRoslynActiveRule);
  }

  @Test
  public void metrics() {
    sensor.analyse(project, context);

    verify(context).saveMeasure(inputFile, CoreMetrics.LINES, 27d);
    verify(context).saveMeasure(inputFile, CoreMetrics.CLASSES, 1d);
    verify(context).saveMeasure(inputFile, CoreMetrics.STATEMENTS, 2d);
    verify(context).saveMeasure(inputFile, CoreMetrics.FUNCTIONS, 3d);
    verify(context).saveMeasure(inputFile, CoreMetrics.PUBLIC_API, 4d);
    verify(context).saveMeasure(inputFile, CoreMetrics.PUBLIC_UNDOCUMENTED_API, 2d);
    verify(context).saveMeasure(inputFile, CoreMetrics.COMPLEXITY, 3d);
  }

  @Test
  public void distribution() {
    sensor.analyse(project, context);

    ArgumentCaptor<Measure> captor = ArgumentCaptor.forClass(Measure.class);
    verify(context, Mockito.times(2)).saveMeasure(Mockito.eq(inputFile), captor.capture());
    int i = 0;
    for (Measure measure : captor.getAllValues()) {
      if (measure.getMetric().equals(CoreMetrics.FILE_COMPLEXITY_DISTRIBUTION)) {
        i++;
        assertThat(measure.getData()).isEqualTo("0=1;5=0;10=0;20=0;30=0;60=0;90=0");
      } else if (measure.getMetric().equals(CoreMetrics.FUNCTION_COMPLEXITY_DISTRIBUTION)) {
        i++;
        assertThat(measure.getData()).isEqualTo("1=3;2=0;4=0;6=0;8=0;10=0;12=0");
      }
    }
    assertThat(i).isEqualTo(2);
  }

  @Test
  public void commentsAndNoSonar() {
    sensor.analyse(project, context);

    verify(noSonarFilter).addComponent(inputFile.key(), ImmutableSet.of(8));
    verify(context).saveMeasure(inputFile, CoreMetrics.COMMENT_LINES, 2d);
  }

  @Test
  public void devCockpit() {
    sensor.analyse(project, context);

    verify(fileLinesContext).setIntValue(CoreMetrics.COMMENT_LINES_DATA_KEY, 3, 1);
    verify(fileLinesContext).setIntValue(CoreMetrics.COMMENT_LINES_DATA_KEY, 7, 1);

    verify(fileLinesContext).setIntValue(CoreMetrics.NCLOC_DATA_KEY, 1, 1);
    verify(fileLinesContext).setIntValue(CoreMetrics.NCLOC_DATA_KEY, 12, 1);
    verify(fileLinesContext).setIntValue(CoreMetrics.NCLOC_DATA_KEY, 13, 1);
  }

  @Test
  public void issue() {
    sensor.analyse(project, context);

    verify(issueBuilder1).ruleKey(RuleKey.of(CSharpPlugin.REPOSITORY_KEY, "S1186"));
    verify(issueBuilder1).message("Add a nested comment explaining why this method is empty, throw an NotSupportedException or complete the implementation.");
    verify(issueBuilder1).line(16);
    verify(issuable).addIssue(issue1);
  }

  @Test
  public void escapesAnalysisInput() throws Exception {
    sensor.analyse(project, context);

    assertThat(
      Files.toString(new File("src/test/resources/CSharpSensorTest/SonarLint.xml"), Charsets.UTF_8).replaceAll("\r?\n|\r", "")
        .replaceAll("<File>.*?Foo&amp;Bar.cs</File>", "<File>Foo&amp;Bar.cs</File>"))
          .isEqualTo(Files.toString(new File("src/test/resources/CSharpSensorTest/SonarLint-expected.xml"), Charsets.UTF_8).replaceAll("\r?\n|\r", ""));
  }

  @Test
  public void testShouldExecute() {
    if (SystemUtils.IS_OS_WINDOWS) {
      assertThat(sensor.shouldExecuteOnProject(project)).isTrue();
    } else {
      assertThat(sensor.shouldExecuteOnProject(project)).isFalse();
    }
  }

  @Test
  public void roslynReportIsProcessed() {
    enableCustomRoslynRules();

    when(settings.getString("sonar.cs.roslyn.reportFilePath")).thenReturn(new File("src/test/resources/CSharpSensorTest/roslyn-report.json").getAbsolutePath());
    sensor.analyse(project, context);

    // We use a mocked rule runner which will report an issue even if a Roslyn report is provided
    verify(issueBuilder1).ruleKey(RuleKey.of(CSharpPlugin.REPOSITORY_KEY, "S1186"));
    verify(issueBuilder1).message("Add a nested comment explaining why this method is empty, throw an NotSupportedException or complete the implementation.");
    verify(issueBuilder1).line(16);

    verify(issueBuilder2).ruleKey(RuleKey.of(CSharpPlugin.REPOSITORY_KEY, "[parameters_key]"));
    verify(issueBuilder2).message("Short messages should be used first in Roslyn reports");
    verify(issueBuilder2).line(43);

    verify(issueBuilder3).ruleKey(RuleKey.of(CSharpPlugin.REPOSITORY_KEY, "[parameters_key]"));
    verify(issueBuilder3).message("There only is a full message in the Roslyn report");
    verify(issueBuilder3).line(1);

    verify(issueBuilder4).ruleKey(RuleKey.of("roslyn.foo", "custom-roslyn"));
    verify(issueBuilder4).message("Custom Roslyn analyzer message");
    verify(issueBuilder4).line(93);

    verify(issueBuilder5).ruleKey(RuleKey.of(CSharpPlugin.REPOSITORY_KEY, "[parameters_key]"));
    verify(issueBuilder5).message("This is an assembly level Roslyn issue with no location");
    verify(issueBuilder5, Mockito.never()).line(1);

    verify(issuable).addIssue(issue1);
    verify(issuable).addIssue(issue2);
    verify(issuable).addIssue(issue3);
    verify(issuable).addIssue(issue4);
    verify(projectIssuable).addIssue(issue5);
  }

  @Test
  public void roslynRulesNotExecutedTwice() throws Exception {
    when(settings.getString("sonar.cs.roslyn.reportFilePath")).thenReturn(new File("src/test/resources/CSharpSensorTest/roslyn-report.json").getAbsolutePath());
    sensor.analyse(project, context);

    assertThat(
      Files.toString(new File("src/test/resources/CSharpSensorTest/SonarLint.xml"), Charsets.UTF_8).replaceAll("\r?\n|\r", "")
        .replaceAll("<File>.*?Foo&amp;Bar.cs</File>", "<File>Foo&amp;Bar.cs</File>"))
          .isEqualTo(Files.toString(new File("src/test/resources/CSharpSensorTest/SonarLint-expected-with-roslyn.xml"), Charsets.UTF_8).replaceAll("\r?\n|\r", ""));
  }

  @Test
  public void roslynEmptyReportShouldNotFail() {
    when(settings.getString("sonar.cs.roslyn.reportFilePath")).thenReturn(new File("src/test/resources/CSharpSensorTest/roslyn-report-empty.json").getAbsolutePath());
    sensor.analyse(project, context);
  }

  @Test
  public void failWithDuplicateRuleKey() {
    enableCustomRoslynRules();

    String ruleKey = parametersActiveRule.getRuleKey();
    when(customRoslynActiveRule.getRuleKey()).thenReturn(ruleKey);

    when(settings.getString("sonar.cs.roslyn.reportFilePath")).thenReturn(new File("src/test/resources/CSharpSensorTest/roslyn-report.json").getAbsolutePath());

    thrown.expectMessage("Rule keys must be unique, but \"[parameters_key]\" is defined in both the \"csharpsquid\" and \"roslyn.foo\" rule repositories.");

    sensor.analyse(project, context);
  }

  @Test
  public void failWithCustomRoslynRulesAndMSBuild12() {
    enableCustomRoslynRules();
    when(settings.getString("sonar.cs.roslyn.reportFilePath")).thenReturn(null);

    thrown.expectMessage(
      "Custom and 3rd party Roslyn analyzers are only by MSBuild 14. Either use MSBuild 14, or disable the custom/3rd party Roslyn analyzers in your quality profile.");
    sensor.analyse(project, context);
  }

}
