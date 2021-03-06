'┌─────┬─────────────────────────────────────────────────────────────┐
'│ 類別名稱 │   上料，過帳記錄查詢                                                                                                     │
'├─────┼──────┬───────┬──────────────────────────────────────────────┤
'│ 日期     │撰寫人      │版本號        │撰寫內容                                                                                    │
'├─────┼──────┼───────┼──────────────────────────────────────────────┤
'│2011/05/27 |  tianen     | 01           │
'├─────┼──────┼───────┼──────────────────────────────────────────────┤
'│2012/10/08│tianen      │1             │修改查詢上料記錄從44表的回焊時間去查詢呢                                                    │
'│2012/11/06│tianen      │2012.10.8.3   │修改44表中沒有回焊時間直接寫過去                                                            │
'│2012/11/28│tianen      │2012.10.8.4   │匯出修改                                                                                    │
'│2012/12/21│tianen      │2012.10.8.5   │add 查出的料卷增加code欄位                                                                  │
'│2013/03/11│tianen      │2012.10.8.6   │增加料卷查板子條碼                                                                          │
'└─────┴──────┴───────┴──────────────────────────────────────────────┘
'2012.10.8.7 修改板子查料卷邏輯
'│2015/06/09│TianEn      │2012.10.8.12  │增加料卷板子(非绑定工单的)                                                                  │
'└─────┴──────┴───────┴──────────────────────────────────────────────┘

Public Class LEXC01
    Public sqlcommand As String = ""
    Dim filename As String

    Dim SumData As DataTable
    Private Sub ToolBar1_ButtonClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolBarButtonClickEventArgs) Handles ToolBar1.ButtonClick
        Select Case e.Button.Text
            Case StrConv("查詢", Me.SpiChangeLanguage.Value, 9)
                If Me.BarLV.Items.Count > 0 Then
                    Me.DataGridView1.DataSource = ""
                    chaxun()
                End If

                'Me.ToolBarButton7.Enabled = True
            Case StrConv("匯入", Me.SpiChangeLanguage.Value, 9)
                BarLV.Items.Clear()
                Me.OpenFileDialog1.InitialDirectory = "d:\"
                Me.OpenFileDialog1.Filter = "EXECLE files (*.xls)|*.xls|All files (*.*)|*.*"
                Me.OpenFileDialog1.FileName = ""
                Me.OpenFileDialog1.ShowDialog()
            Case StrConv("匯出", Me.SpiChangeLanguage.Value, 9)
                If Me.DataGridView1.Rows.Count > 0 Then
                    Save()
                    'Update()
                End If
            Case StrConv("結束", Me.SpiChangeLanguage.Value, 9)
                Me.Close()
        End Select
    End Sub
    'Private Sub updata()
    '    DataGridView1.DataSource = ""
    '    BarLV.Items.Clear()
    '    StatusBar1.Panels(0).Text = ""

    'End Sub
    Private Sub chaxun()
        Me.StatusBar1.Panels(0).Text = "查询开始"
        If RadioButton1.Checked Then
            input_check()
        End If

        If RadioButton2.Checked Then
            pass_check()
        End If

        If RadioButton3.Checked Then
            barcode_check()
        End If

        If RadioButton4.Checked Then
            ReelNo_check()
        End If

        Me.StatusBar1.Panels(0).Text = "查询结束"
    End Sub
    Private Sub ReelNo_check()
        Try
            SumData = New DataTable

            For i As Integer = 0 To BarLV.Items.Count - 1
                'sqlcommand = "Select pdline.pdline_name,smt.in_time,smt.out_time From smt.g_smt_travel smt,sajet.sys_pdline pdline " & _
                '             " Where pdline.pdline_id=smt.pdline " & _
                '             " and smt.reel_no='" & BarLV.Items(i).SubItems(0).Text & "'"
                sqlcommand = "select m11.f001 reflowsid, x.*" & _
                             "  from (Select (select msl.part_id" & _
                             "                  from smt.g_msl msl" & _
                             "                where msl.msl_no = smt.msl_no" & _
                             "                   and rownum = 1) part_id," & _
                             "                pdline.pdline_name," & _
                             "                smt.in_time," & _
                             "                smt.out_time" & _
                             "          From smt.g_smt_travel smt, sajet.sys_pdline pdline" & _
                             "         Where pdline.pdline_id = smt.pdline" & _
                             "           and smt.reel_no = '" & BarLV.Items(i).SubItems(0).Text & "') x," & _
                             "       mse0011 m11" & _
                             " where m11.f002 = x.part_id" & _
                             "   and m11.f004 = 6" 

                Dim dt As New DataTable
                Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt)
                If dt.Rows.Count > 0 Then
                    For ii As Integer = 0 To dt.Rows.Count - 1
                        Dim LineNo As String = dt.Rows(ii).Item("pdline_name")
                        Dim ReflowSid As Integer = dt.Rows(ii).Item("reflowsid")
                        Dim PartSid As Integer = dt.Rows(ii).Item("part_id")

                        'sqlcommand = "select '" & BarLV.Items(i).SubItems(0).Text & "' as reelno,f003 as barcode," & _
                        '             "'" & LineNo & "' as lineno,'" & dt.Rows(ii).Item(1) & "' as in_time,'" & dt.Rows(ii).Item(2) & "' as out_time" & _
                        '             " from mse0060 where substr(f012,1,3)='" & LineNo & "'" & _
                        '             " and f005 between To_Date('" & dt.Rows(ii).Item(1) & "', 'YYYY/MM/DD HH24:MI:SS') And" & _
                        '             " To_Date('" & dt.Rows(ii).Item(2) & "', 'YYYY/MM/DD HH24:MI:SS')"
                        sqlcommand = "select '" & BarLV.Items(i).SubItems(0).Text & "' as reelno, f003 as barcode,f005 as reflowtime," & _
                                     "'" & LineNo & "' as lineno,'" & dt.Rows(ii).Item("in_time") & "' as in_time,'" & dt.Rows(ii).Item("out_time") & "' as out_time" & _
                                     "  from mse0060" & _
                                     " where f004 = " & ReflowSid & _
                                     "   and f002 = " & PartSid & _
                                     "   and substr(f012, 1, 3) = '" & LineNo & "'" & _
                                     "   AND (F005 BETWEEN" & _
                                     "             TO_DATE('" & dt.Rows(ii).Item("in_time") & "', 'YYYY/MM/DD HH24:MI:SS') AND" & _
                                     "             TO_DATE('" & dt.Rows(ii).Item("out_time") & "', 'YYYY/MM/DD hh24:mi:ss'))"
                        Dim dt1 As New DataTable
                        Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt1)
                        If dt1.Rows.Count > 0 Then
                            SumData.Merge(dt1)
                        End If
                    Next
               
                End If
            Next
            Me.DataGridView1.DataSource = SumData

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub barcode_check()
        '’‘’‘’根據料卷查打在那些板子上
        Try
            SumData = New DataTable
            Dim barcode As String
            For i As Integer = 0 To BarLV.Items.Count - 1
                barcode = Trim(BarLV.Items(i).SubItems(0).Text)

                sqlcommand = "select se17.f001,se17.f005,smt.in_time,smt.out_time" & _
                             " from smt.g_smt_travel smt, mse0017 se17 " & _
                             " where smt.reel_no = '" & barcode & "' " & _
                             " and smt.wo = se17.f003"
                Dim dt1 As New DataTable
                Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt1)
                If dt1.Rows.Count > 0 Then
                    For x As Integer = 0 To dt1.Rows.Count - 1
                        sqlcommand = "select '" & barcode & "'reel, se44.f002 barcode " & _
                                     " from mse0044 se44 " & _
                                     " where se44.worknosid =" & dt1.Rows(x).Item(0) & " " & _
                                     " and workflow_time between TO_DATE ('" & dt1.Rows(x).Item(2) & "','YYYY/MM/DD HH24:MI:SS') " & _
                                     " And TO_DATE ('" & dt1.Rows(x).Item(3) & "','YYYY/MM/DD HH24:MI:SS') " & _
                                     " and se44.f001 = " & dt1.Rows(x).Item(1) & ""
                        Dim dt As New DataTable
                        Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt)
                        If dt.Rows.Count > 0 Then
                            SumData.Merge(dt)
                        End If
                        dt.Dispose()
                    Next
                End If
                dt1.Dispose()
            Next

            Me.DataGridView1.DataSource = SumData
            Me.StatusBar1.Panels(0).Text = StrConv("查詢完成", Me.SpiChangeLanguage.Value, 9)
            'Me.ToolBarButton4.Enabled = False

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub pass_check()
        '**************
        '*查詢過帳記錄*
        '**************
        Try
            SumData = New DataTable
            Dim barcode As String
            Dim k As Integer
            For k = 0 To BarLV.Items.Count - 1
                barcode = Trim(BarLV.Items(k).SubItems(0).Text)
                sqlcommand = "select se08.f002 from mse0044 se44,mse0008 se08 where se44.f002='" & barcode & "' and se44.f001=se08.f001"
                Dim dt1 As New DataTable
                Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt1)
                If dt1.Rows.Count > 0 Then
                    sqlcommand = "select '" & barcode & "' barcode,a.* from table(viewbarcodehistory.history1('" & dt1.Rows(0).Item("f002") & "','" & barcode & "')) a order by 4 "
                    Dim dt2 As DataTable = New DataTable("dt")
                    Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt2)
                    If dt2.Rows.Count > 0 Then
                        SumData.Merge(dt2)
                    End If
                End If
            Next
            Me.DataGridView1.DataSource = SumData
            Me.StatusBar1.Panels(0).Text = StrConv("查詢完成", Me.SpiChangeLanguage.Value, 9)
            'Me.ToolBarButton4.Enabled = False
        Catch ex As Exception
            ErrorInfo.SysErrMessageInfomation(StrConv(ex.Message, Me.SpiChangeLanguage.Value, 9))
        End Try
    End Sub
    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        filename = Me.OpenFileDialog1.FileName
        Me.OpenFileDialog1.Dispose()
        Dim app As Object = CreateObject("Excel.Application") '定義excel實例
        Dim xlbook As Object = app.WorkBooks.Open(filename) '打開已經存在的工作薄
        Dim xlsheet As Object = xlbook.worksheets(1) '設置xlsheet為工作表1
        Try
            xlsheet.Activate() '設置xlsheet為當前工作表
            Dim numR As Integer = xlsheet.usedrange.rows.count
            Dim numC As Integer = xlsheet.usedrange.columns.count
            Dim num As Integer = 0
            Dim part_id As Integer = 0
            If numC <> 1 Then
                xlbook.close()
                app.Quit()
                app = Nothing
                GC.Collect()
                MsgBox("文檔格式有誤，請檢查")
                Exit Sub
            End If
            'If numR > 50 Then
            '    xlbook.close()
            '    app.Quit()
            '    app = Nothing
            '    GC.Collect()
            '    MsgBox("最多只允許查50個條碼")
            '    Exit Sub
            'End If
            Dim i As Integer = 0
            Dim j As Integer = 0
            For i = 1 To numR
                num = BarLV.Items.Count
                BarLV.Items.Insert(num, Trim(xlsheet.cells(i, 1).value))
            Next
            xlbook.close()
            app.Quit()
            app = Nothing
            GC.Collect()
            'Me.ToolBarButton4.Enabled = True
            'Me.ToolBarButton6.Enabled = False
        Catch ex As Exception
            xlbook.close()
            app.Quit()
            app = Nothing
            GC.Collect()
        End Try
    End Sub
    Private Sub Save()

        Dim app As Object = CreateObject("Excel.Application")
        Dim xlbook As Object = app.workbooks.add()
        Dim maxR As Integer = 60000   '設定輸出最大行數，不能超過65535
        Dim xR As Integer = SumData.Rows.Count
        Dim xC As Integer = SumData.Columns.Count
        Dim PageMax As Integer = Int(xR / maxR)
        If CInt(xR / maxR + 0.5) > PageMax Then
            PageMax += 1
        End If
        Dim i As Integer = 0
        'Dim xlExcelApp As Object = CreateObject("Excel.Application")
        'xlExcelApp.WORKBOOKS.ADD()

        'xlExcelApp.Sheets("Sheet1").select()
        'xlExcelApp.sheets("sheet1").Name = StrConv("查詢", Me.SpiChangeLanguage.Value, 9)
        'If DataGridView1.Columns.Count > 0 Then

        '    For J As Integer = 0 To DataGridView1.ColumnCount - 1
        '        xlExcelApp.Cells(1, J + 1).Value = DataGridView1.Columns(J).HeaderText

        '    Next

        '    For I As Integer = 0 To DataGridView1.Rows.Count - 1
        '        For J As Integer = 0 To DataGridView1.ColumnCount - 1
        '            xlExcelApp.Cells(I + 2, J + 1).Value = "'" & DataGridView1.Rows(I).Cells(J).Value
        '        Next
        '    Next

        'End If

        For j As Integer = 1 To PageMax   '分頁輸出
            Dim xlsheet As Object = xlbook.worksheets.add()
            If PageMax > 1 Then
                xlsheet.name = "明細" & j
            Else
                xlsheet.name = "明細"
            End If

            xlsheet.activate()

            '求剩余行數
            If j = PageMax Then
                maxR = xR - maxR * (PageMax - 1)
            End If
            Dim rawData1(maxR, xC - 1) As Object
            For col As Integer = 0 To xC - 1
                rawData1(0, col) = SumData.Columns(col).ColumnName
            Next
            For row As Integer = 0 To maxR - 1
                For col As Integer = 0 To xC - 1
                    rawData1(row + 1, col) = SumData.Rows(row).ItemArray(col)

                Next
                i += 1
            Next
            xlsheet.range(xlsheet.cells(1, 1), xlsheet.cells(maxR + 1, xC)).value2 = rawData1
            xlsheet.Columns.AutoFit()
        Next



        ' ****************
        ' * 設定字體參數 *
        ' ****************
        With app.cells
            .font.Name = "vendana"
            .font.Size = 10
            .HorizontalAlignment = -4108       ' 水平居中
            .VerticalAlignment = -4108         ' 垂直居中
            .Columns.AutoFit()
        End With
        app.visible = True
        app.Quit()
        DataGridView1.Columns.Clear()
        'Me.ToolBarButton7.Enabled = False
        'Me.ToolBarButton6.Enabled = True
    End Sub

    Sub input_check()
        '*****************
        '**查詢上料記錄***
        '*****************
        SumData = New DataTable
        Dim barcode As String
        Dim k As Integer
        For k = 0 To BarLV.Items.Count - 1
            barcode = BarLV.Items(k).SubItems(0).Text.Trim
            If barcode = "" Then
                Exit For
            End If
            Try
                sqlcommand = "SELECT   m60.f012 runcard," & _
                             "         TO_CHAR (m60.f005, 'yyyy/mm/dd hh24:mi:ss') workflowtime," & _
                             "         m08.f002 partno" & _
                             "  FROM   mse0060 m60, mse0008 m08" & _
                             " WHERE   m08.f001 = m60.f002" & _
                             "     AND m60.f003 = '" & barcode & "'" & _
                             "     AND m60.f012 IS NOT NULL"
                Dim dt As New DataTable
                Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt)

                If dt.Rows.Count > 0 Then

                    Dim workflowtime As String = dt.Rows(0).Item("workflowtime")
                    Dim runcard As String = dt.Rows(0).Item("runcard")
                    Dim partno As String = dt.Rows(0).Item("partno")

                    sqlcommand = "SELECT pdline_id FROM SAJET.G_SN_STATUS WHERE SERIAL_NUMBER='" & runcard & "'"
                    dt = New DataTable
                    Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt)

                    If dt.Rows.Count > 0 Then
                        Dim pdline As Integer = dt.Rows(0).Item("pdline_id")

                        sqlcommand = "SELECT   '" & barcode & "' barcode," & _
                                     "         se08.f002 partno," & _
                                     "         b.part_sn reel," & _
                                     "         b.lot_no bin," & _
                                     "         b.used_flag status," & _
                                     "         a.slot_no slot_no," & _
                                     "         c.emp_name emp," & _
                                     "         to_char(a.in_time,'yyyy/mm/dd hh24:mi:ss') in_time," & _
                                     "         to_char(a.out_time,'yyyy/mm/dd hh24:mi:ss') out_time," & _
                                     "         b.vendor_lotno," & _
                                     "         b.qty," & _
                                     "         b.REEL_DAYCODE DC" & _
                                     "  FROM   smt.g_smt_travel a," & _
                                     "         sajet.g_part_map b," & _
                                     "         sajet.sys_emp c," & _
                                     "         mse0008 se08" & _
                                     " WHERE   a.msl_no LIKE '" & partno & "%'" & _
                                     "         AND a.in_time < to_date('" & workflowtime & "','yyyy/mm/dd hh24:mi:ss')" & _
                                     "         AND to_date('" & workflowtime & "','yyyy/mm/dd hh24:mi:ss')< a.out_time" & _
                                     "         AND a.reel_no = b.part_sn" & _
                                     "         AND c.emp_id = a.emp_id" & _
                                     "         AND se08.f001 = b.part_id" & _
                                     "         AND A.PDLINE=" & pdline

                        dt = New DataTable
                        Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt)
                        If dt.Rows.Count > 0 Then
                            SumData.Merge(dt)
                        End If
                    End If

                End If
            Catch ex As Exception
                ErrorInfo.SysErrMessageInfomation(StrConv(ex.Message, Me.SpiChangeLanguage.Value, 9))
            End Try
        Next
        Me.DataGridView1.DataSource = SumData

        Me.StatusBar1.Panels(0).Text = StrConv("查詢完成", Me.SpiChangeLanguage.Value, 9)
        'Me.ToolBarButton4.Enabled = False
        'Me.ToolBarButton6.Enabled = True

    End Sub

    'Sub input_check()
    '    '*****************
    '    '**查詢上料記錄***
    '    '*****************
    '    SumData = New DataTable
    '    Dim barcode As String
    '    Dim k As Integer
    '    For k = 0 To BarLV.Items.Count - 1
    '        barcode = BarLV.Items(k).SubItems(0).Text.Trim
    '        If barcode = "" Then
    '            Exit For
    '        End If
    '        Try
    '            sqlcommand = "select se08.f002,se44.workflow_time from mse0044 se44,mse0008 se08 where se44.f002='" & barcode & "' and se44.f001=se08.f001"

    '            Dim dt As New DataTable
    '            Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt)


    '            If dt.Rows.Count > 0 Then

    '                If Len(IIf(IsDBNull(dt.Rows(0).Item("workflow_time")), "", dt.Rows(0).Item("workflow_time"))) = 0 Then
    '                    sqlcommand = "update mse0044 set workflow_time=(" & _
    '                                 "select f005 from mse0060 where f003='" & barcode & "' and f012 is not null) " & _
    '                                 " where f002='" & barcode & "'"
    '                    Me.ClsDBInfo.ExecuteSQL(sqlcommand)

    '                End If

    '                sqlcommand = ""
    '                sqlcommand = "SELECT '" & barcode & "' barcode,se08.f002 PARTNO,b.part_sn REEL, b.lot_no BIN, b.used_flag STATUS, a.slot_no SLOT_NO, " & _
    '                              "c.emp_name EMP,a.in_time, a.out_time, b.vendor_lotno, b.qty,b.reel_daycode,b.decode" & _
    '                             " FROM smt.g_smt_travel a, + b, sajet.sys_emp c," & _
    '                              " mse0008 se08 WHERE a.msl_no LIKE '" & dt.Rows(0).Item("f002") & "%" & "' " & _
    '                             " AND a.in_time < (SELECT workflow_time  FROM mse0044 WHERE f002 = '" & barcode & "') AND  " & _
    '                             " (SELECT workflow_time FROM mse0044 WHERE f002 = '" & barcode & "') < a.out_time " & _
    '                             " AND a.reel_no = b.part_sn  AND c.emp_id = a.emp_id and se08.f001=b.PART_ID"

    '                Dim dt2 As DataTable = New DataTable("dt")
    '                Me.ClsDBInfo.ExecuteSQL(sqlcommand, dt2)
    '                If dt2.Rows.Count > 0 Then
    '                    SumData.Merge(dt2)
    '                End If
    '            End If
    '        Catch ex As Exception
    '            ErrorInfo.SysErrMessageInfomation(StrConv(ex.Message, Me.SpiChangeLanguage.Value, 9))
    '        End Try
    '    Next
    '    Me.DataGridView1.DataSource = SumData

    '    Me.StatusBar1.Panels(0).Text = StrConv("查詢完成", Me.SpiChangeLanguage.Value, 9)
    '    Me.ToolBarButton4.Enabled = False
    '    Me.ToolBarButton6.Enabled = True

    'End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If Me.TextBox1.Text.Trim.Length > 0 Then
            Me.BarLV.Items.Add(Me.TextBox1.Text.Trim)
            Me.TextBox1.Text = ""
        End If
    End Sub

    Private Sub LEXC01_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class