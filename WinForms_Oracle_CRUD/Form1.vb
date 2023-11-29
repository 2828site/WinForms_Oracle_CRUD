Imports Oracle.ManagedDataAccess.Client

'參考說明
'https://itsakura.com/vbnet-oracle-insert
Public Class Form1

    Dim conn As New OracleConnection("Data Source=192.168.56.101/orclpdb1;Persist Security Info=True;User ID=system;Password=oracle")
    Dim i As Integer
    Dim dr As OracleDataReader

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DGV_load()
    End Sub

    Private Sub btn_save_Click(sender As Object, e As EventArgs) Handles btn_save.Click
        save()
        DGV_load()
    End Sub

    Public Sub save()
        Try
            conn.Open()
            Dim cmd As New OracleCommand("INSERT INTO crud (PRODUCTNO, PRODUCTNAME, PRICE, GROUP1, EXPDATE, STATUS) VALUES  (:PRODUCTNO, :PRODUCTNAME, :PRICE, :GROUP1, :EXPDATE, :STATUS)", conn)
            cmd.Parameters.Clear()
            cmd.Parameters.Add(":PRODUCTNO", txt_prono.Text)
            cmd.Parameters.Add(":PRODUCTNAME", txt_proname.Text)
            cmd.Parameters.Add(":PRICE", CDec(txt_price.Text))
            cmd.Parameters.Add(":GROUP1", combo_proGroup.Text)
            cmd.Parameters.Add(":EXPDATE", CDate(exp_datepicker.Value))
            If status_checkbox.CheckState = 1 Then
                cmd.Parameters.Add(":STATUS", 1)
            Else
                cmd.Parameters.Add(":STATUS", 0)
            End If

            i = cmd.ExecuteNonQuery
            If i > 0 Then
                MessageBox.Show("Record Save Success !", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show("Record Save Failed !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Public Sub DGV_load()
        DataGridView1.Rows.Clear()
        Try
            conn.Open()
            Dim cmd As New OracleCommand("SELECT * FROM crud", conn)
            dr = cmd.ExecuteReader
            While dr.Read
                DataGridView1.Rows.Add(dr.Item("PRODUCTNO"), dr.Item("PRODUCTNAME"), dr.Item("PRICE"), dr.Item("GROUP1"), dr.Item("EXPDATE"), Format(CBool(dr.Item("STATUS"))))
            End While
            dr.Close()
            dr.Dispose()
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
        clear()
    End Sub

    Public Sub clear()
        txt_prono.Clear()
        txt_proname.Clear()
        txt_price.Clear()
        combo_proGroup.Text = ""
        exp_datepicker.Value = Now
        status_checkbox.CheckState = False
        DataGridView1.ClearSelection()
        txt_prono.ReadOnly = False
        btn_save.Enabled = True
        btn_save.BackColor = Color.ForestGreen
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        txt_prono.Text = DataGridView1.CurrentRow.Cells(0).Value
        txt_proname.Text = DataGridView1.CurrentRow.Cells(1).Value
        txt_price.Text = DataGridView1.CurrentRow.Cells(2).Value
        combo_proGroup.Text = DataGridView1.CurrentRow.Cells(3).Value
        exp_datepicker.Text = DataGridView1.CurrentRow.Cells(4).Value
        status_checkbox.Checked = DataGridView1.CurrentRow.Cells(5).Value

        txt_prono.ReadOnly = True
        btn_save.Enabled = False
        btn_save.BackColor = Color.DimGray
    End Sub

    Public Sub edit()
        Try
            conn.Open()
            Dim transaction As OracleTransaction = conn.BeginTransaction()
            Dim cmd As New OracleCommand("UPDATE crud SET PRODUCTNAME=:PRODUCTNAME, PRICE=:PRICE, GROUP1=:GROUP1, EXPDATE=:EXPDATE, STATUS=:STATUS WHERE PRODUCTNO=:PRODUCTNO", conn)

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.BindByName = True

            cmd.Parameters.Clear()
            'cmd.Parameters.Add(New OracleParameter("PRODUCTNO", OracleDbType.Varchar2)).Value = "133526"
            'cmd.Parameters.Add(New OracleParameter("PRODUCTNAME", OracleDbType.Varchar2)).Value = "Test VB"

            cmd.Parameters.Add(":PRODUCTNO", txt_prono.Text)
            cmd.Parameters.Add(":PRODUCTNAME", txt_proname.Text)
            cmd.Parameters.Add(":PRICE", CDec(txt_price.Text))
            cmd.Parameters.Add(":GROUP1", combo_proGroup.Text)
            cmd.Parameters.Add(":EXPDATE", CDate(exp_datepicker.Value))
            If status_checkbox.CheckState = 1 Then
                cmd.Parameters.Add(":STATUS", 1)
            Else
                cmd.Parameters.Add(":STATUS", 0)
            End If

            i = cmd.ExecuteNonQuery
            transaction.Commit()

            If i > 0 Then
                MessageBox.Show("Record Update Success !", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show("Record Update Failed !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
        clear()
        DGV_load()
    End Sub

    Private Sub btn_edit_Click(sender As Object, e As EventArgs) Handles btn_edit.Click
        edit()
    End Sub

    Public Sub delete()
        If MsgBox("Are You Sure Delete This Record", MsgBoxStyle.Question + vbYesNo) = vbYes Then
            Try
                conn.Open()
                Dim cmd As New OracleCommand("DELETE FROM crud WHERE PRODUCTNO=:PRODUCTNO", conn)
                cmd.Parameters.Clear()
                cmd.Parameters.Add(":PRODUCTNO", txt_prono.Text)
                i = cmd.ExecuteNonQuery
                If i > 0 Then
                    MessageBox.Show("Record Delete Success !", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("Record Delete Failed !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                conn.Close()
            End Try
            clear()
            DGV_load()
        End If
    End Sub

    Private Sub btn_delete_Click(sender As Object, e As EventArgs) Handles btn_delete.Click
        delete()
    End Sub

    Private Sub btn_clear_Click(sender As Object, e As EventArgs) Handles btn_clear.Click
        clear()
    End Sub

    Private Sub txt_search_TextChanged(sender As Object, e As EventArgs) Handles txt_search.TextChanged
        DataGridView1.Rows.Clear()
        Try
            conn.Open()
            Dim cmd As New OracleCommand("SELECT * FROM crud WHERE PRODUCTNO like '%" & txt_search.Text & "%' OR PRODUCTNAME like '%" & txt_search.Text & "%'", conn)
            dr = cmd.ExecuteReader
            While dr.Read
                DataGridView1.Rows.Add(dr.Item("PRODUCTNO"), dr.Item("PRODUCTNAME"), dr.Item("PRICE"), dr.Item("GROUP1"), dr.Item("EXPDATE"), Format(CBool(dr.Item("STATUS"))))
            End While
            dr.Close()
            dr.Dispose()
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
        clear()
    End Sub

End Class
