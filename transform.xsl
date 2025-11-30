<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <html>
            <head>
                <style>
                    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f3f2f1; padding: 20px; }
                    table { border-collapse: collapse; width: 100%; background-color: white; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }
                    th, td { border: 1px solid #d4d4d4; padding: 12px; text-align: left; font-size: 14px; }
                    th { background-color: #217346; color: white; font-weight: bold; }
                    tr:nth-child(even) { background-color: #f9f9f9; }
                    h2 { color: #217346; border-bottom: 2px solid #217346; padding-bottom: 10px; }
                    h3 { color: #444; margin-top: 30px; }
                    ul { list-style-type: none; padding: 0; }
                    li { background: white; margin: 5px 0; padding: 10px; border: 1px solid #d4d4d4; border-left: 5px solid #217346; }
                </style>
            </head>
            <body>
                <h2>Звіт: Спорт на факультеті</h2>
                <h3>Спортивні секції та учасники</h3>
                <table>
                    <tr><th>Секція</th><th>Тренер</th><th>Тип учасника</th><th>ПІБ</th><th>Розряд</th><th>Деталі</th></tr>
                    <xsl:for-each select="FacultySports/Section/Participant">
                        <tr>
                            <td><xsl:value-of select="../@name"/></td>
                            <td><xsl:value-of select="../@coach"/></td>
                            <td><xsl:value-of select="@type"/></td>
                            <td><xsl:value-of select="@fullName"/></td>
                            <td>
                                <xsl:choose>
                                    <xsl:when test="@rank"><xsl:value-of select="@rank"/></xsl:when>
                                    <xsl:otherwise>-</xsl:otherwise>
                                </xsl:choose>
                            </td>
                            <td>
                                <xsl:choose>
                                    <xsl:when test="@group">Група: <xsl:value-of select="@group"/></xsl:when>
                                    <xsl:when test="@position">Посада: <xsl:value-of select="@position"/></xsl:when>
                                    <xsl:otherwise>-</xsl:otherwise>
                                </xsl:choose>
                            </td>
                        </tr>
                    </xsl:for-each>
                </table>
                <h3>План змагань</h3>
                <ul>
                    <xsl:for-each select="FacultySports/CompetitionPlan/Event">
                        <li><b><xsl:value-of select="@name"/></b> (<xsl:value-of select="@date"/>) - <xsl:value-of select="@type"/></li>
                    </xsl:for-each>
                </ul>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>