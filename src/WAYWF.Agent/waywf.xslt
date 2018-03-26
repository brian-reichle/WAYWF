<?xml version="1.0" encoding="utf-8" ?>
<!-- Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information. -->

<xsl:stylesheet
	version="1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:w="waywf-capture"
	xmlns:svg="http://www.w3.org/2000/svg"
	xmlns:xlink="http://www.w3.org/1999/xlink"
	>
	<xsl:output method="xml" />
	<xsl:variable name="hexDigits">0123456789ABCDEF</xsl:variable>

	<xsl:key name="moduleLookup" match="/w:waywf/w:process/w:appDomain/w:assembly/w:module" use="@moduleId" />
	<xsl:key name="documentLookup" match="/w:waywf/w:process/w:source/w:document" use="@documentId" />
	<xsl:key name="threadLookup" match="/w:waywf/w:process/w:thread" use="@tid" />
	<xsl:key name="valueLookup" match="/w:waywf/w:process/w:value|/w:waywf/w:process/w:rcwValue" use="@objectId" />
	<xsl:key name="windowLookup" match="/w:waywf/w:process/w:window" use="@ownerThread" />
	<xsl:key name="windowByOwner" match="/w:waywf/w:process/w:window[@visible='true']" use="@ownerHwnd" />
	<xsl:key name="pendingSMTaskByModule" match="/w:waywf/w:process/w:pendingTasks/w:pendingSMTask[@state &gt;= 0]" use="@moduleId" />

	<xsl:template match="/">
		<html>
			<head>
				<title>
					<xsl:text>Process: </xsl:text>
					<xsl:value-of select="w:waywf/w:process/@pid" />
					<xsl:text> - </xsl:text>
					<xsl:call-template name="filename">
						<xsl:with-param name="sub" select="w:waywf/w:process/@imagePath" />
					</xsl:call-template>
				</title>
				<script type="text/javascript">
					<![CDATA[
var currentElement = null;

function setHilighted(newElement)
{
	if (currentElement != null)
	{
		currentElement.classList.remove('hilight');
	}

	currentElement = newElement;

	if (currentElement != null)
	{
		currentElement.classList.add('hilight');
	}
}

function hilightByhash()
{
	var hash = document.location.hash.substring(1);
	setHilighted(document.getElementById(hash));
}
					]]>
				</script>
				<style>
					<![CDATA[
h1 { font-size: 140%; }
h2 { font-size: 120%; }

th
{
	color: White;
	background-color: Navy;
}

th, td
{
	text-align: left;
	vertical-align: text-top;
}

table, td, th
{
	border-style: solid;
	border-width: thin;
	border-color: Black;
	border-collapse: collapse;
}

table
{
	width: 95%;
	background-color: White;
}

.paramKnownValue, .localKnownValue, .hasSource { background-color: lightgray; }
.hilight { background-color: yellow; }

.localContainer
{
	float: right;
	width: 16px;
	height: 16px;
}

.localContainer .drop
{
	height: 0;
	overflow: hidden;
	position: relative;
	z-index: 1;
	float: right;
	top: -50%;
}

.localContainer:hover .drop
{
	overflow: visible;
}

				]]>
				</style>
				<svg:svg>
					<defs>
						<svg:g id="LocalsIcon">
							<svg:circle cx="9" cy="7" r="5" stroke="black" stroke-width="2" fill="transparent" />
							<svg:line x1="1" y1="15" x2="6" y2="10" stroke="black" stroke-width="2" />
						</svg:g>
					</defs>
				</svg:svg>
			</head>
			<body onhashchange="hilightByhash();">
				<xsl:apply-templates select="w:waywf/w:process" />
				<xsl:apply-templates select="w:waywf/w:process" mode="TargetSection" />
				<xsl:apply-templates select="w:waywf" mode="ContextSection" />
			</body>
		</html>
	</xsl:template>

	<xsl:template match="w:process" mode="TargetSection">
		<h1>Target</h1>

		<b>Process:</b>
		<xsl:text> </xsl:text>
		<xsl:value-of select="@pid"/>
		<xsl:text> - </xsl:text>
		<xsl:call-template name="filename">
			<xsl:with-param name="sub" select="@imagePath" />
		</xsl:call-template>
		<xsl:text> (</xsl:text>
		<xsl:choose>
			<xsl:when test="/w:waywf/w:os/@is64bit = 'false'">
				<xsl:text>32 bit</xsl:text>
			</xsl:when>
			<xsl:when test="@is64bit = 'false'">
				<xsl:text>Wow64</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>64 bit</xsl:text>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:text>)</xsl:text>
		<br />

		<xsl:for-each select="w:user">
			<b>User:</b>
			<xsl:text> </xsl:text>
			<xsl:value-of select="@user" />
			<xsl:text>/</xsl:text>
			<xsl:value-of select="@domain" />
			<br />
		</xsl:for-each>

		<b>CLR Version:</b>
		<xsl:text> </xsl:text>
		<xsl:value-of select="@clrVersion"/>
		<br />
	</xsl:template>

	<xsl:template match="w:waywf" mode="ContextSection">
		<h1>Context</h1>

		<b>Time:</b>
		<xsl:text> </xsl:text>
		<xsl:value-of select="@timestamp" />
		<xsl:text> </xsl:text>
		<xsl:value-of select="@timezone" />
		<xsl:if test="@wait">
			<xsl:text> (wait </xsl:text>
			<xsl:value-of select="@wait" />
			<xsl:text> seconds)</xsl:text>
		</xsl:if>
		<br />

		<b>Operating System Version:</b>
		<xsl:text> </xsl:text>
		<xsl:value-of select="w:os/@platform" />
		<xsl:text> </xsl:text>
		<xsl:value-of select="w:os/@version" />
		<xsl:text> </xsl:text>
		<xsl:value-of select="w:os/@servicePack" />
		<br />

		<xsl:for-each select="w:login">
			<b>Login:</b>
			<xsl:text> </xsl:text>
			<xsl:value-of select="@user" />
			<xsl:text>/</xsl:text>
			<xsl:value-of select="@domain" />
			<xsl:text> (on </xsl:text>
			<xsl:value-of select="@machine"/>
			<xsl:text>)</xsl:text>
			<br />
		</xsl:for-each>

		<b>WAYWF Version:</b>
		<xsl:text> </xsl:text>
		<xsl:value-of select="@version" />
		<br />
	</xsl:template>

	<xsl:template match="w:process">
		<xsl:apply-templates select="." mode="windows" />
		<xsl:apply-templates select="." mode="threads" />

		<xsl:if test="/w:waywf/@walkheap = 'true'">
			<xsl:apply-templates select="." mode="pendingtasks" />
		</xsl:if>

		<xsl:apply-templates select="." mode="assemblies" />
	</xsl:template>

	<xsl:template match="w:process" mode="threads">
		<h1>Threads</h1>
		<table>
			<thead>
				<tr>
					<th>Thread Id</th>
					<th>State</th>
					<th>Flags</th>
					<th />
					<th>Location</th>
				</tr>
			</thead>
			<tbody>
				<xsl:apply-templates select="w:thread" mode="row" />
			</tbody>
		</table>
		<xsl:apply-templates select="w:thread[w:chain/w:frame]" />
	</xsl:template>

	<xsl:template match="w:process" mode="pendingtasks">
		<h1>Pending Tasks</h1>
		<table>
			<thead>
				<tr>
					<th>Module</th>
					<th>Async Method</th>
					<th>State</th>
				</tr>
			</thead>
			<tbody>
				<xsl:apply-templates select="w:pendingTasks/w:pendingSMTask[generate-id() = generate-id(key('pendingSMTaskByModule', @moduleId)[1])]" />
			</tbody>
		</table>
	</xsl:template>

	<xsl:template match="w:process" mode="windows">
		<xsl:if test="count(w:window[@visible='true'])">
			<h1>Windows</h1>
			<table>
				<thead>
					<tr>
						<th>HWND</th>
						<th>Caption</th>
						<th>Flags</th>
						<th>Owning Thread</th>
					</tr>
				</thead>
				<tbody>
					<xsl:apply-templates select="w:window[@visible='true' and not(@ownerHwnd)]">
						<xsl:sort select="@ownerThread" />
					</xsl:apply-templates>
				</tbody>
			</table>
		</xsl:if>
	</xsl:template>

	<xsl:template match="w:process" mode="assemblies">
		<h1>Assemblies</h1>
		<xsl:apply-templates select="w:appDomain" />
	</xsl:template>

	<xsl:template match="w:thread" mode="row">
		<tr>
			<td>
				<xsl:call-template name="threadLink">
					<xsl:with-param name="tid" select="@tid" />
				</xsl:call-template>
			</td>
			<td>
				<xsl:choose>
					<xsl:when test="contains(@state, 'USER_UNSTARTED')">Unstarted</xsl:when>
					<xsl:when test="contains(@state, 'USER_STOP_REQUESTED')">Stopping</xsl:when>
					<xsl:when test="contains(@state, 'USER_STOPPED')">Stopped</xsl:when>
					<xsl:when test="contains(@state, 'USER_SUSPEND_REQUESTED')">Suspending</xsl:when>
					<xsl:when test="contains(@state, 'USER_SUSPENDED')">Suspended</xsl:when>
					<xsl:when test="contains(@state, 'USER_WAIT_SLEEP_JOIN')">Blocked</xsl:when>
					<xsl:otherwise>Running</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>
				<xsl:if test="contains(@state, 'USER_THREADPOOL')">
					<span title="Threadpool Thread.">T</span>
				</xsl:if>
				<xsl:if test="contains(@state, 'USER_BACKGROUND')">
					<span title="Background Thread.">B</span>
				</xsl:if>
				<xsl:if test="key('windowLookup', @tid)[@visible='true']">
					<span title="Has a visible window handle.">W</span>
				</xsl:if>
				<xsl:if test="key('windowLookup', @tid)[not(@visible='true')]">
					<span title="Has a non-visible window handle.">H</span>
				</xsl:if>
			</td>
			<td>
				<xsl:variable name="maxDuration" select="(w:chain/w:frame[@duration])[last()]/@duration" />
				<xsl:call-template name="barBackground">
					<xsl:with-param name="value" select="$maxDuration" />
					<xsl:with-param name="max" select="/w:waywf/@wait" />
				</xsl:call-template>
				<xsl:value-of select="$maxDuration" />
			</td>
			<td>
				<xsl:variable name="sourcedFrame" select="(w:chain/w:frame[w:source])[1]" />

				<xsl:choose>
					<xsl:when test="$sourcedFrame">
						<xsl:apply-templates select="$sourcedFrame" mode="light" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:apply-templates select="(w:chain/w:frame)[1]" mode="light" />
					</xsl:otherwise>
				</xsl:choose>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="w:thread">
		<h2>
			<a id="{generate-id(.)}">
				<xsl:value-of select="@tid" />
				<xsl:text> (</xsl:text>
				<xsl:call-template name="formatHex">
					<xsl:with-param name="value" select="@tid" />
				</xsl:call-template>
				<xsl:text>)</xsl:text>
			</a>
		</h2>

		<xsl:if test="w:blockingObject">
			<table>
				<thead>
					<tr>
						<th>Block Type</th>
						<th>Timeout</th>
						<th>Owner</th>
						<th>Lock Object</th>
					</tr>
				</thead>
				<tbody>
					<xsl:apply-templates select="w:blockingObject" />
				</tbody>
			</table>
			<br />
		</xsl:if>

		<table>
			<tbody>
				<xsl:apply-templates select="w:chain/w:frame|w:chain/w:internalFrame" />
			</tbody>
		</table>
	</xsl:template>

	<xsl:template match="w:frame" mode="light">
		<xsl:call-template name="filename">
			<xsl:with-param name="sub" select="key('moduleLookup', @moduleId)/@path" />
		</xsl:call-template>
		<xsl:text>!</xsl:text>
		<xsl:if test="@typeDisplayText">
			<xsl:value-of select="@typeDisplayText" />
			<xsl:text>.</xsl:text>
		</xsl:if>
		<xsl:value-of select="@methodDisplayText" />
		<xsl:text>(</xsl:text>
		<xsl:for-each select="w:param">
			<xsl:if test="position() != 1">
				<xsl:text>, </xsl:text>
			</xsl:if>
			<xsl:value-of select="@type" />
			<xsl:if test="@name">
				<xsl:text> </xsl:text>
				<xsl:value-of select="@name" />
			</xsl:if>
		</xsl:for-each>
		<xsl:text>)</xsl:text>
	</xsl:template>

	<xsl:template match="w:frame">
		<xsl:variable name="currentThread" select="ancestor::w:thread" />
		<xsl:variable name="previousFrame" select="(preceding::w:frame[generate-id(./ancestor::w:thread) = generate-id($currentThread)]|preceding::w:internalFrame)[last()]" />
		<tr>
			<xsl:if test="not($previousFrame/@moduleId) or $previousFrame/@moduleId != @moduleId">
				<xsl:variable name="currentFrame" select="." />
				<xsl:variable name="startOfNextGroup" select="(following::w:frame[@moduleId != $currentFrame/@moduleId or generate-id(./ancestor::w:thread) != generate-id($currentThread)]|following::w:internalFrame)[1]" />
				<xsl:variable name="set1" select=".|following::w:frame" />
				<xsl:variable name="set2" select="$startOfNextGroup/preceding::w:frame" />
				<xsl:variable name="groupNodes" select="$set1[not($startOfNextGroup) or count($set2) = count($set2 | .)]" />

				<td rowspan="{count($groupNodes)}">
					<a href="#{generate-id(key('moduleLookup', @moduleId))}">
						<xsl:call-template name="filename">
							<xsl:with-param name="sub" select="key('moduleLookup', @moduleId)/@path" />
						</xsl:call-template>
					</a>
				</td>
			</xsl:if>
			<td>
				<xsl:call-template name="barBackground">
					<xsl:with-param name="value" select="@duration" />
					<xsl:with-param name="max" select="/w:waywf/@wait" />
				</xsl:call-template>
				<xsl:value-of select="@duration" />
			</td>
			<td>
				<xsl:apply-templates select="." mode="locals" />
				<xsl:if test="@typeDisplayText">
					<span>
						<xsl:if test="w:this/w:value|w:this/w:valueRef|w:this/w:rcwValue|w:this/w:pointerValue|w:this/w:null">
							<xsl:attribute name="class">paramKnownValue</xsl:attribute>
							<xsl:attribute name="title">
								<xsl:apply-templates select="w:this/w:value|w:this/w:valueRef|w:this/w:rcwValue|w:this/w:pointerValue|w:this/w:null" />
							</xsl:attribute>
						</xsl:if>
						<xsl:value-of select="@typeDisplayText" />
					</span>
					<xsl:text>.</xsl:text>
				</xsl:if>
				<xsl:value-of select="@methodDisplayText" />
				<xsl:text>(</xsl:text>
				<xsl:for-each select="w:param">
					<xsl:if test="position() != 1">
						<xsl:text>, </xsl:text>
					</xsl:if>
					<xsl:apply-templates select="." />
				</xsl:for-each>
				<xsl:text>)</xsl:text>
				<xsl:if test="@ilOffset">
					<xsl:text> </xsl:text>
					<a>
						<xsl:if test="w:source">
							<xsl:attribute name="class">hasSource</xsl:attribute>
							<xsl:attribute name="title">
								<xsl:apply-templates select="w:source" />
							</xsl:attribute>
							<xsl:attribute name="href">
								<xsl:apply-templates select="w:source" mode="url" />
							</xsl:attribute>
						</xsl:if>
						<xsl:text>+</xsl:text>
						<xsl:value-of select="@ilOffset" />
					</a>
				</xsl:if>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="w:frame|w:pendingSMTask" mode="locals">
		<xsl:if test="w:local/w:value|w:local/valueRef|w:local/w:rcwValue|w:local/w:pointerValue|w:local/w:null">
			<div class="localContainer">
				<svg:svg width="16" height="16">
					<svg:use xlink:href="#LocalsIcon" />
				</svg:svg>
				<div class="drop">
					<table>
						<thead>
							<tr>
								<th>Locals</th>
							</tr>
						</thead>
						<tbody>
							<xsl:for-each select="w:local">
								<tr>
									<td>
										<xsl:apply-templates select="." />
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</div>
			</div>
		</xsl:if>
	</xsl:template>

	<xsl:template match="w:internalFrame">
		<tr>
			<td colspan="3">
				<xsl:choose>
					<xsl:when test=". = 'STUBFRAME_M2U'">[Managed to Unmanaged Transition]</xsl:when>
					<xsl:when test=". = 'STUBFRAME_U2M'">[Unmanaged to Managed Transition]</xsl:when>
					<xsl:when test=". = 'STUBFRAME_APPDOMAIN_TRANSITION'">[AppDomain Transition]</xsl:when>
					<xsl:when test=". = 'STUBFRAME_LIGHTWEIGHT_FUNCTION'">[Light Weight Function]</xsl:when>
					<xsl:when test=". = 'STUBFRAME_INTERNALCALL'">[Internal Call]</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="." />
					</xsl:otherwise>
				</xsl:choose>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="w:pendingSMTask">
		<xsl:variable name="tasks" select="key('pendingSMTaskByModule', @moduleId)" />

		<xsl:for-each select="$tasks">
			<tr>
				<xsl:if test="position() = 1">
					<td rowspan="{count($tasks)}">
						<xsl:variable name="moduleId" select="substring-before(concat(normalize-space(@moduleId),' '),' ')" />

						<a href="#{generate-id(key('moduleLookup', $moduleId))}">
							<xsl:call-template name="filename">
								<xsl:with-param name="sub" select="key('moduleLookup', $moduleId)/@path" />
							</xsl:call-template>
						</a>
					</td>
				</xsl:if>
				<td>
					<xsl:apply-templates select="." mode="locals" />
					<xsl:if test="@typeDisplayText">
						<xsl:value-of select="@typeDisplayText" />
						<xsl:text>.</xsl:text>
					</xsl:if>
					<xsl:value-of select="@methodDisplayText" />
					<xsl:text>(</xsl:text>
					<xsl:for-each select="w:param">
						<xsl:if test="position() != 1">
							<xsl:text>, </xsl:text>
						</xsl:if>
						<xsl:apply-templates select="." />
					</xsl:for-each>
					<xsl:text>)</xsl:text>
					<xsl:if test="@ilOffset">
						<xsl:text> </xsl:text>
						<a>
							<xsl:if test="w:source">
								<xsl:attribute name="class">hasSource</xsl:attribute>
								<xsl:attribute name="title">
									<xsl:apply-templates select="w:source" />
								</xsl:attribute>
								<xsl:attribute name="href">
									<xsl:apply-templates select="w:source" mode="url" />
								</xsl:attribute>
							</xsl:if>
							<xsl:text>+</xsl:text>
							<xsl:value-of select="@ilOffset" />
						</a>
					</xsl:if>
				</td>
				<td>
					<xsl:value-of select="@state" />
				</td>
			</tr>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="w:param|w:local">
		<span>
			<xsl:variable name="value" select="w:value|w:valueRef|w:rcwValue|w:pointerValue|w:null" />
			<xsl:if test="$value">
				<xsl:attribute name="class">
					<xsl:value-of select="local-name(.)" />
					<xsl:text>KnownValue</xsl:text>
				</xsl:attribute>
				<xsl:attribute name="title">
					<xsl:apply-templates select="$value" />
				</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="translate(@type,' ','&#160;')" />
			<xsl:if test="@byRef = 'true'">
				<xsl:text>&amp;</xsl:text>
			</xsl:if>
			<xsl:if test="@name">
				<xsl:text>&#160;</xsl:text>
				<xsl:value-of select="@name" />
			</xsl:if>
		</span>
	</xsl:template>

	<xsl:template match="w:null">
		<xsl:text>null</xsl:text>
	</xsl:template>

	<xsl:template match="w:value">
		<xsl:if test="@objectId">
			<xsl:value-of select="@objectId" />
			<xsl:text>: </xsl:text>
		</xsl:if>
		<xsl:value-of select="@type" />
		<xsl:if test="string-length(.) != 0">
			<xsl:text>(</xsl:text>
			<xsl:value-of select="." />
			<xsl:text>)</xsl:text>
		</xsl:if>
	</xsl:template>

	<xsl:template match="w:valueRef">
		<xsl:apply-templates select="key('valueLookup', @objectId)" />
	</xsl:template>

	<xsl:template match="w:rcwValue">
		<xsl:if test="@objectId">
			<xsl:value-of select="@objectId" />
			<xsl:text>: </xsl:text>
		</xsl:if>
		<xsl:value-of select="@type" />
	</xsl:template>

	<xsl:template match="w:pointerValue">
		<xsl:value-of select="@type" />
		<xsl:text>(</xsl:text>
		<xsl:value-of select="@address" />
		<xsl:text>)</xsl:text>
	</xsl:template>

	<xsl:template match="w:source">
		<xsl:value-of select="key('documentLookup', @documentId)/ancestor::w:source/@path" />
		<xsl:value-of select="key('documentLookup', @documentId)" />
		<xsl:text> line:</xsl:text>
		<xsl:value-of select="@line" />
	</xsl:template>

	<xsl:template match="w:source" mode="url">
		<xsl:text>file://</xsl:text>
		<xsl:value-of select="key('documentLookup', @documentId)/ancestor::w:source/@path" />
		<xsl:value-of select="key('documentLookup', @documentId)" />
	</xsl:template>

	<xsl:template match="w:window">
		<xsl:param name="indent" select="0" />
		<tr>
			<td>
				<div style="margin-left: {$indent}px; display: inline;">
					<xsl:value-of select="@hwnd" />
					<xsl:text> (</xsl:text>
					<xsl:call-template name="formatHex">
						<xsl:with-param name="value" select="@hwnd" />
					</xsl:call-template>
					<xsl:text>)</xsl:text>
				</div>
			</td>
			<td>
				<xsl:value-of select="." />
			</td>
			<td>
				<xsl:if test="@enabled != 'true'">
					<span title="Disabled.">D</span>
				</xsl:if>
				<xsl:if test="not(@visible='true')">
					<span title="Hidden">H</span>
				</xsl:if>
			</td>
			<td>
				<xsl:call-template name="threadLink">
					<xsl:with-param name="tid" select="@ownerThread" />
				</xsl:call-template>
			</td>
		</tr>
		<xsl:apply-templates select="key('windowByOwner', @hwnd)">
			<xsl:with-param name="indent" select="$indent + 20" />
			<xsl:sort select="@ownerThread" />
		</xsl:apply-templates>
	</xsl:template>

	<xsl:template match="w:blockingObject">
		<tr>
			<td>
				<xsl:choose>
					<xsl:when test="@reason = 'BLOCKING_MONITOR_CRITICAL_SECTION'">Enter</xsl:when>
					<xsl:when test="@reason = 'BLOCKING_MONITOR_EVENT'">Wait</xsl:when>
					<xsl:otherwise>Unknown</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>
				<xsl:choose>
					<xsl:when test="@timeout">
						<xsl:value-of select="@timeout" />
						<xsl:text>ms</xsl:text>
					</xsl:when>
					<xsl:otherwise>Infinite</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>
				<xsl:call-template name="threadLink">
					<xsl:with-param name="tid" select="@ownerThread" />
				</xsl:call-template>
			</td>
			<td>
				<xsl:apply-templates select="w:value|w:valueRef|w:rcwValue" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="w:appDomain">
		<h2>
			<xsl:value-of select="@id" />
			<xsl:text> - </xsl:text>
			<xsl:value-of select="@name" />
		</h2>

		<table>
			<thead>
				<tr>
					<th>Assembly Name</th>
					<th>Version</th>
					<th>Public Key Token</th>
					<th>Module Name</th>
				</tr>
			</thead>
			<tbody>
				<xsl:apply-templates select="w:assembly">
					<xsl:sort select="@name" />
				</xsl:apply-templates>
			</tbody>
		</table>
	</xsl:template>

	<xsl:template match="w:assembly">
		<xsl:variable name="assembly" select="." />
		<xsl:variable name="moduleCount" select="count(w:module)" />
		<xsl:for-each select="w:module">
			<xsl:sort select="@name" />
			<tr>
				<xsl:if test="position() = 1">
					<td rowspan="{$moduleCount}">
						<xsl:value-of select="$assembly/@name" />
					</td>
					<td rowspan="{$moduleCount}">
						<xsl:value-of select="$assembly/@version" />
					</td>
					<td rowspan="{$moduleCount}">
						<xsl:value-of select="$assembly/@publicKeyToken" />
					</td>
				</xsl:if>
				<td>
					<a id="{generate-id(.)}">
						<xsl:attribute name="title">
							<xsl:choose>
								<xsl:when test="@isInMemory = 'true'">&lt;in memory&gt;</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="@path" />
								</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						<xsl:value-of select="@name" />
					</a>
				</td>
			</tr>
		</xsl:for-each>
	</xsl:template>

	<xsl:template name="threadLink">
		<xsl:param name="tid" />
		<xsl:choose>
			<xsl:when test="key('threadLookup', $tid)/w:chain/w:frame">
				<a href="#{generate-id(key('threadLookup', $tid))}">
					<xsl:value-of select="$tid" />
				</a>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$tid" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="filename">
		<xsl:param name="value" />
		<xsl:param name="sub" select="substring-after($value, '\')" />
		<xsl:choose>
			<xsl:when test="$sub">
				<xsl:call-template name="filename">
					<xsl:with-param name="value" select="$sub" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$value" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="barBackground">
		<xsl:param name="value" />
		<xsl:param name="max" />
		<xsl:if test="$value and $max">
			<xsl:variable name="percentage" select="($value div $max) * 100" />
			<xsl:attribute name="style">
				<xsl:text>background: </xsl:text>
				<xsl:text>linear-gradient(90deg, lavender, lavender </xsl:text>
				<xsl:value-of select="$percentage" />
				<xsl:text>%, transparent </xsl:text>
				<xsl:value-of select="$percentage" />
				<xsl:text>%, transparent</xsl:text>
				<xsl:text>)</xsl:text>
				<xsl:text>;</xsl:text>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>

	<xsl:template name="formatHex">
		<xsl:param name="value" />
		<xsl:if test="$value &gt;= 16">
			<xsl:call-template name="formatHex">
				<xsl:with-param name="value" select="floor($value div 16)" />
			</xsl:call-template>
		</xsl:if>
		<xsl:value-of select="substring($hexDigits, ($value mod 16) + 1, 1)" />
	</xsl:template>
</xsl:stylesheet>
