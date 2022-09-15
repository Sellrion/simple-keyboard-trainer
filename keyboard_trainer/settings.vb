Imports System.Collections.Generic

Public Class settings

    Public OPTIONS As New Dictionary(Of String, Object)

    Sub New()
        InitializeComponent()

        Me.OPTIONS.Add("use_russian_alphabet", True)
        Me.OPTIONS.Add("use_english_alphabet", True)
        Me.OPTIONS.Add("use_numeric_keys", True)
        Me.OPTIONS.Add("use_cursor_control_keys", False)
        Me.OPTIONS.Add("use_functional_keys", False)
        Me.OPTIONS.Add("use_phrases", False)
        Me.OPTIONS.Add("use_misc_symbols", False)
        Me.OPTIONS.Add("use_key_shortcuts", False)
        Me.OPTIONS.Add("use_time_ranges", False)
        Me.OPTIONS.Add("one_task_time", 5)
        Me.OPTIONS.Add("one_phrasetask_time", 15)
        Me.OPTIONS.Add("test_time", 75)
        Me.OPTIONS.Add("tasks_consitency", 1)
        Me.OPTIONS.Add("tasks_consitency_tasks_to_show", 15)
        Me.OPTIONS.Add("use_marks", False)
        Me.OPTIONS.Add("give_one_attempt_per_task", False)
        Me.OPTIONS.Add("mark_five_persentage", {100, 90})
        Me.OPTIONS.Add("mark_four_persentage", {89, 70})
        Me.OPTIONS.Add("mark_three_persentage", {69, 50})
    End Sub

    Private Sub settings_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'Dialog setup
        Me.CheckBox1.Checked = Me.OPTIONS("use_russian_alphabet")
        Me.CheckBox2.Checked = Me.OPTIONS("use_english_alphabet")
        Me.CheckBox3.Checked = Me.OPTIONS("use_numeric_keys")
        Me.CheckBox4.Checked = Me.OPTIONS("use_misc_symbols")
        Me.CheckBox5.Checked = Me.OPTIONS("use_cursor_control_keys")
        Me.CheckBox6.Checked = Me.OPTIONS("use_functional_keys")
        Me.CheckBox7.Checked = Me.OPTIONS("use_phrases")
        Me.CheckBox10.Checked = Me.OPTIONS("use_key_shortcuts")

        Me.CheckBox8.Checked = Me.OPTIONS("use_time_ranges")
        Me.NumericUpDown1.Value = Me.OPTIONS("one_task_time")
        Me.NumericUpDown10.Value = Me.OPTIONS("one_phrasetask_time")
        Me.NumericUpDown2.Value = Me.OPTIONS("test_time")
        Me.Label1.Enabled = Me.CheckBox8.Checked
        Me.Label2.Enabled = Me.CheckBox8.Checked
        Me.Label16.Enabled = Me.CheckBox8.Checked
        Me.NumericUpDown1.Enabled = Me.CheckBox8.Checked
        Me.NumericUpDown2.Enabled = Me.CheckBox8.Checked
        Me.NumericUpDown10.Enabled = Me.CheckBox8.Checked

        If Me.OPTIONS("tasks_consitency") = 0 Then
            Me.RadioButton1.Checked = True
            Me.RadioButton2.Checked = False
        ElseIf Me.OPTIONS("tasks_consitency") = 1 Then
            Me.RadioButton1.Checked = False
            Me.RadioButton2.Checked = True
        End If
        Me.NumericUpDown3.Value = Me.OPTIONS("tasks_consitency_tasks_to_show")

        Me.CheckBox11.Checked = Me.OPTIONS("give_one_attempt_per_task")

        Me.Panel1.Enabled = Me.OPTIONS("use_marks")

        Me.NumericUpDown4.Value = Me.OPTIONS("mark_five_persentage")(0)
        Me.NumericUpDown5.Value = Me.OPTIONS("mark_five_persentage")(1)
        Me.NumericUpDown7.Value = Me.OPTIONS("mark_four_persentage")(0)
        Me.NumericUpDown6.Value = Me.OPTIONS("mark_four_persentage")(1)
        Me.NumericUpDown9.Value = Me.OPTIONS("mark_three_persentage")(0)
        Me.NumericUpDown8.Value = Me.OPTIONS("mark_three_persentage")(1)

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles RadioButton2.CheckedChanged
        Me.Label3.Enabled = Me.RadioButton2.Checked
        Me.NumericUpDown3.Enabled = Me.RadioButton2.Checked
    End Sub

    Private Sub CheckBox8_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox8.CheckedChanged
        Me.Label1.Enabled = Me.CheckBox8.Checked
        Me.Label2.Enabled = Me.CheckBox8.Checked
        Me.Label16.Enabled = Me.CheckBox8.Checked
        Me.NumericUpDown1.Enabled = Me.CheckBox8.Checked
        Me.NumericUpDown2.Enabled = Me.CheckBox8.Checked
        Me.NumericUpDown10.Enabled = Me.CheckBox8.Checked
        If Me.CheckBox8.Checked = False Then
            Me.RadioButton1.Enabled = False
            Me.RadioButton2.Checked = True
        Else
            Me.RadioButton1.Enabled = True
        End If
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        'Saving options
        If Me.CheckBox1.Checked = False And Me.CheckBox2.Checked = False And Me.CheckBox3.Checked = False And Me.CheckBox4.Checked = False And Me.CheckBox5.Checked = False And Me.CheckBox6.Checked = False And Me.CheckBox7.Checked = False And Me.CheckBox10.Checked = False Then
            MsgBox("Необходимо выбрать хотя бы один элемент в состав заданий теста.", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Настройки")
            Exit Sub
        End If
        If Me.CheckBox9.Checked = True Then
            If Me.NumericUpDown4.Value < Me.NumericUpDown5.Value Or Me.NumericUpDown7.Value < Me.NumericUpDown6.Value Or Me.NumericUpDown9.Value < Me.NumericUpDown8.Value Then
                MsgBox("Нижняя граница одного или нескольких процентных диапазонов критериев оценки выше верхней границы.", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Настройки")
                Exit Sub
            End If
            If Me.NumericUpDown5.Value < Me.NumericUpDown7.Value Or Me.NumericUpDown6.Value < Me.NumericUpDown9.Value Then
                MsgBox("Границы процентных диапазонов критериев оценки перекрываются.", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Настройки")
                Exit Sub
            End If
            If ((Me.NumericUpDown4.Value - Me.NumericUpDown5.Value) + (Me.NumericUpDown7.Value - Me.NumericUpDown6.Value) + (Me.NumericUpDown9.Value - Me.NumericUpDown8.Value)) >= 100 Then
                MsgBox("Общее число процентов в процентных диапазонах критериев оценки не может быть больше или равным 100.", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Настройки")
                Exit Sub
            End If
        End If

        'All are fine - save the options
        Me.OPTIONS("use_russian_alphabet") = Me.CheckBox1.Checked
        Me.OPTIONS("use_english_alphabet") = Me.CheckBox2.Checked
        Me.OPTIONS("use_numeric_keys") = Me.CheckBox3.Checked
        Me.OPTIONS("use_misc_symbols") = Me.CheckBox4.Checked
        Me.OPTIONS("use_cursor_control_keys") = Me.CheckBox5.Checked
        Me.OPTIONS("use_functional_keys") = Me.CheckBox6.Checked
        Me.OPTIONS("use_phrases") = Me.CheckBox7.Checked
        Me.OPTIONS("use_key_shortcuts") = Me.CheckBox10.Checked
        Me.OPTIONS("use_time_ranges") = Me.CheckBox8.Checked
        Me.OPTIONS("one_task_time") = Me.NumericUpDown1.Value
        Me.OPTIONS("one_phrasetask_time") = Me.NumericUpDown10.Value
        Me.OPTIONS("test_time") = Me.NumericUpDown2.Value
        If Me.RadioButton1.Checked = True Then
            Me.OPTIONS("tasks_consitency") = 0
            Me.OPTIONS("tasks_consitency_tasks_to_show") = Me.NumericUpDown3.Value
        ElseIf Me.RadioButton2.Checked = True Then
            Me.OPTIONS("tasks_consitency") = 1
            Me.OPTIONS("tasks_consitency_tasks_to_show") = Me.NumericUpDown3.Value
        End If
        Me.OPTIONS("use_marks") = Me.CheckBox9.Checked

        Me.OPTIONS("give_one_attempt_per_task") = Me.CheckBox11.Checked

        Me.OPTIONS("mark_five_persentage")(0) = Me.NumericUpDown4.Value
        Me.OPTIONS("mark_five_persentage")(1) = Me.NumericUpDown5.Value
        Me.OPTIONS("mark_four_persentage")(0) = Me.NumericUpDown7.Value
        Me.OPTIONS("mark_four_persentage")(1) = Me.NumericUpDown6.Value
        Me.OPTIONS("mark_three_persentage")(0) = Me.NumericUpDown9.Value
        Me.OPTIONS("mark_three_persentage")(1) = Me.NumericUpDown8.Value

        Me.Close()
    End Sub

    Private Sub CheckBox9_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox9.CheckedChanged
        Me.Panel1.Enabled = Me.CheckBox9.Checked
    End Sub
End Class