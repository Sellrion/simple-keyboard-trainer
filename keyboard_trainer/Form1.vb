Public Class Form1
    Public SETTINGS As New settings
    Public SUMMARY As New summary
    Public test_inprogress As Boolean
    Public test_sec As Integer
    Public task_sec As Integer
    Public task_sec_const As Integer
    Public phrasetask_sec As Integer
    Public phrasetask_sec_const As Integer
    Public RUSSIAN_ALPHABET(33) As Object
    Public ENGLISH_ALPHABET(26) As Object
    Public CYPHERS(10) As Object
    Public MISC_SYMBOLS(20) As Object
    Public FUNCTIONAL_KEYS(12) As Object
    Public CURSOR_CONTROL_KEYS(16) As Object
    Public KEY_SHORTCUTS(15) As Object
    Public Structure KEY
        Public name As String
        Public symbol As String
        Public keycode As Integer
        Public layout_needed As String
        Public using_shift As Boolean
        Public using_ctrl As Boolean
        Public using_alt As Boolean
    End Structure
    Public PHRASES(50) As String
    Public test_stopwatch_on As Boolean
    Public task_stopwatch_on As Boolean
    Public Structure test_answer
        Public tsk_type As Integer
        Public key_needed As KEY
        Public key_pressed As KEY
        Public phrase_needed As String
        Public phrase_entered As String
        Public seconds_spent As Integer
    End Structure
    Public KEY_NEEDED As New KEY
    Public PHRASE_NEEDED As String
    Public needed_keytype As Integer
    Public ANSWERS() As Object
    Public task_number As Integer
    Public tasks_left As Integer
    Public shift_isdown As Boolean
    Public alt_isdown As Boolean
    Public ctrl_isdown As Boolean
    Public testtime_is_elapsed As Boolean
    Public viewing_mark As Boolean

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'Setup interface layout
        Me.Panel1.Visible = False
        Me.Panel3.Visible = False
        Me.Panel5.Visible = False
        Me.Panel7.Visible = False
        Me.Panel8.Visible = False
        Me.Panel9.Visible = False
        Dim a As New Point
        a.X = 0
        a.Y = (Me.Height - Me.Panel1.Height) / 2
        Me.Panel1.Location = a
        Me.Panel1.Width = Me.Width
        a.X = (Me.Panel1.Width - Me.Panel2.Width) / 2
        a.Y = 0
        Me.Panel2.Location = a
        a.X = Me.Width - Me.Panel5.Width
        a.Y = 0
        Me.Panel5.Location = a
        Me.Panel7.Width = (Me.Width / 100) * 60
        a.X = (Me.Width / 2) - (Me.Panel7.Width / 2)
        a.Y = Me.Height - Me.Panel7.Height
        Me.Panel7.Location = a
        a.Y = 13
        a.X = 0
        Me.Label6.Location = a
        a.Y = 13
        a.X = Me.Panel7.Width - Me.Label7.Width
        Me.Label7.Location = a
        a.X = (Me.Width / 2) - (Me.Panel8.Width / 2)
        a.Y = (Me.Height / 2) - (Me.Panel8.Height / 2)
        Me.Panel8.Location = a
        Me.Panel9.Width = Me.Width
        a.X = 0
        a.Y = (Me.Height / 2) - (Me.Panel9.Height / 2)
        Me.Panel9.Location = a
        a.Y = 0
        a.X = (Me.Panel9.Width / 2) - (Me.Panel10.Width / 2)
        Me.Panel10.Location = a

        'Define symbol collections
        SymbolCollections_Init()

        'Detect current unput language
        Select Case InputLanguage.CurrentInputLanguage.LayoutName
            Case "Русская"
                Me.Label7.Text = "RU"
            Case "США"
                Me.Label7.Text = "EN"
            Case "Британская"
                Me.Label7.Text = "EN"
            Case Else
                Me.Label7.Text = "N/A"
        End Select
        test_inprogress = False
        Me.Panel1.Visible = True
        Randomize()
    End Sub

    Private Sub keylogger_keydown(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        'Block Caps Lock handling
        If e.KeyCode = 20 And test_inprogress = True Then
            Exit Sub
        End If
        'Handling settings change
        If e.Alt = True And e.Control = True And e.KeyCode = 83 And test_inprogress = False And viewing_mark = False Then
            SETTINGS.ShowDialog()
            Exit Sub
        End If
        'Halt current test session and start the new one
        If e.Alt = True And e.Control = True And e.KeyCode = 66 And test_inprogress = True Then
            test_inprogress = False
            Timer1.Stop()
            Me.Panel8.Visible = False
            Me.Panel5.Visible = False
            Me.Panel3.Visible = False
            Me.Panel7.Visible = False
            Me.Panel1.Visible = True
            Exit Sub
        End If
        If test_inprogress = True And Not needed_keytype = 5 Then
            If e.Shift = True Then
                shift_isdown = True
            End If
            If e.Alt = True Then
                alt_isdown = True
            End If
            If e.Control = True Then
                ctrl_isdown = True
            End If
        End If
    End Sub

    Private Sub keylogger_keyup(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        'Block CapsLock handling
        If e.KeyCode = 20 And test_inprogress = True Then
            Exit Sub
        End If
        If test_inprogress = True Then
            If Not needed_keytype = 5 Then
                If e.KeyCode = 16 Or e.KeyCode = 17 Or e.KeyCode = 18 Then
                    shift_isdown = False
                    alt_isdown = False
                    ctrl_isdown = False
                    Exit Sub
                End If
                If shift_isdown = KEY_NEEDED.using_shift And ctrl_isdown = KEY_NEEDED.using_ctrl And alt_isdown = KEY_NEEDED.using_alt And e.KeyCode = KEY_NEEDED.keycode And (Not InStr(KEY_NEEDED.layout_needed, InputLanguage.CurrentInputLanguage.LayoutName) = 0 Or KEY_NEEDED.layout_needed = "any") Then
                    Dim answer As New test_answer
                    answer.key_needed = KEY_NEEDED
                    answer.key_pressed.keycode = e.KeyCode
                    answer.key_pressed.using_shift = shift_isdown
                    answer.key_pressed.using_ctrl = ctrl_isdown
                    answer.key_pressed.using_alt = alt_isdown
                    answer.key_pressed.layout_needed = InputLanguage.CurrentInputLanguage.LayoutName
                    answer.tsk_type = needed_keytype
                    answer.seconds_spent = task_sec_const - task_sec
                    ANSWERS(task_number - tasks_left) = answer
                    shift_isdown = False
                    alt_isdown = False
                    ctrl_isdown = False
                    tasks_left = tasks_left - 1
                    colordelay(1, False, 1)
                    next_task()
                    Exit Sub
                Else
                    If SETTINGS.OPTIONS("give_one_attempt_per_task") = False Then
                        colordelay(0, True, 1)
                        shift_isdown = False
                        alt_isdown = False
                        ctrl_isdown = False
                        Exit Sub
                    Else
                        Dim answer As New test_answer
                        answer.key_needed = KEY_NEEDED
                        answer.key_pressed.keycode = e.KeyCode
                        answer.key_pressed.using_shift = shift_isdown
                        answer.key_pressed.using_ctrl = ctrl_isdown
                        answer.key_pressed.using_alt = alt_isdown
                        answer.key_pressed.layout_needed = InputLanguage.CurrentInputLanguage.LayoutName
                        answer.tsk_type = needed_keytype
                        answer.seconds_spent = task_sec_const - task_sec
                        ANSWERS(task_number - tasks_left) = answer
                        shift_isdown = False
                        alt_isdown = False
                        ctrl_isdown = False
                        tasks_left = tasks_left - 1
                        colordelay(0, False, 1)
                        next_task()
                        Exit Sub
                    End If
                End If
            Else
                If e.KeyCode = Keys.Enter Then
                    If Me.TextBox1.Text = PHRASE_NEEDED Then
                        Dim answer As New test_answer
                        answer.phrase_needed = PHRASE_NEEDED
                        answer.phrase_entered = Me.TextBox1.Text
                        answer.tsk_type = needed_keytype
                        answer.seconds_spent = task_sec_const - task_sec
                        ANSWERS(task_number - tasks_left) = answer
                        tasks_left = tasks_left - 1
                        colordelay(1, False, 1)
                        next_task()
                        Exit Sub
                    Else
                        If SETTINGS.OPTIONS("give_one_attempt_per_task") = False Then
                            colordelay(0, True, 1)
                            Exit Sub
                        Else
                            Dim answer As New test_answer
                            answer.phrase_needed = PHRASE_NEEDED
                            answer.phrase_entered = Me.TextBox1.Text
                            answer.tsk_type = needed_keytype
                            answer.seconds_spent = task_sec_const - task_sec
                            ANSWERS(task_number - tasks_left) = answer
                            tasks_left = tasks_left - 1
                            colordelay(0, False, 1)
                            next_task()
                            Exit Sub
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub keylogger_keypress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
        If Not needed_keytype = 5 Then
            e.Handled = True
        End If
    End Sub

    Private Sub inputlang_changed(sender As System.Object, e As System.Windows.Forms.InputLanguageChangedEventArgs) Handles Me.InputLanguageChanged
        Select Case e.InputLanguage.LayoutName
            Case "Русская"
                Me.Label7.Text = "RU"
            Case "США"
                Me.Label7.Text = "EN"
            Case "Британская"
                Me.Label7.Text = "EN"
            Case Else
                Me.Label7.Text = "N/A"
        End Select
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        End
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Dim ab As New about
        ab.ShowDialog()
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Dim min, sec As Integer
        Dim smin, ssec As String
        'For test time
        If test_stopwatch_on = True Then
            If Not test_sec = 0 Then
                test_sec = test_sec - 1
                If test_sec < 60 Then
                    If test_sec < 10 Then Me.Label3.Text = "00:0" + test_sec.ToString Else Me.Label3.Text = "00:" + test_sec.ToString
                Else
                    min = Math.Floor(test_sec / 60)
                    sec = test_sec - (min * 60)
                    If min < 10 Then smin = "0" + min.ToString Else smin = min.ToString
                    If sec < 10 Then ssec = "0" + sec.ToString Else ssec = sec.ToString
                    Me.Label3.Text = smin + ":" + ssec
                End If
            Else
                testtime_is_elapsed = True
                colordelay(0, False, 1)
                give_mark()
            End If
        End If
        'For one task time
        If task_stopwatch_on = True Then
            If Not task_sec = 0 Then
                task_sec = task_sec - 1
                If task_sec < 60 Then
                    If task_sec < 10 Then Me.Label5.Text = "00:0" + task_sec.ToString Else Me.Label5.Text = "00:" + task_sec.ToString
                Else
                    min = Math.Floor(task_sec / 60)
                    sec = task_sec - (min * 60)
                    If min < 10 Then smin = "0" + min.ToString Else smin = min.ToString
                    If sec < 10 Then ssec = "0" + sec.ToString Else ssec = sec.ToString
                    Me.Label5.Text = smin + ":" + ssec
                End If
            Else
                tasks_left = tasks_left - 1
                colordelay(0, False, 1)
                next_task()
            End If
        End If
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        'Figure out how many tasks do we have
        If SETTINGS.OPTIONS("use_time_ranges") = False Then
            task_number = SETTINGS.OPTIONS("tasks_consitency_tasks_to_show")
            test_sec = 0
            task_sec_const = 0
            phrasetask_sec_const = 0
            test_stopwatch_on = False
            task_stopwatch_on = False
        Else
            If SETTINGS.OPTIONS("tasks_consitency") = 0 Then
                task_number = Math.Floor(SETTINGS.OPTIONS("test_time") / SETTINGS.OPTIONS("one_task_time"))
                test_sec = SETTINGS.OPTIONS("test_time")
                task_sec_const = SETTINGS.OPTIONS("one_task_time")
                phrasetask_sec_const = SETTINGS.OPTIONS("one_phrasetask_time")
                test_stopwatch_on = True
                task_stopwatch_on = True
            Else
                task_number = SETTINGS.OPTIONS("tasks_consitency_tasks_to_show")
                test_sec = SETTINGS.OPTIONS("test_time")
                task_sec_const = 0
                phrasetask_sec_const = SETTINGS.OPTIONS("one_phrasetask_time")
                test_stopwatch_on = True
                task_stopwatch_on = False
            End If
        End If
        tasks_left = task_number
        Me.Label6.Text = "Осталось заданий " & tasks_left & " из " & task_number
        ReDim ANSWERS(task_number)
        Dim min, sec As Integer
        Dim smin, ssec As String
        If test_stopwatch_on = True Then
            If test_sec < 60 Then
                If test_sec < 10 Then ssec = "0" + test_sec.ToString Else ssec = test_sec.ToString
                Me.Label3.Text = "00:" + ssec
            Else
                min = Math.Floor(test_sec / 60)
                sec = test_sec - (min * 60)
                If min < 10 Then smin = "0" + min.ToString Else smin = min.ToString
                If sec < 10 Then ssec = "0" + sec.ToString Else ssec = sec.ToString
                Me.Label3.Text = smin + ":" + ssec
            End If
        Else
            Me.Label3.Text = "00:00"
        End If
        If task_stopwatch_on = True And task_sec_const > 0 Then
            If task_sec_const < 60 Then
                If task_sec_const < 10 Then ssec = "0" + task_sec_const.ToString Else ssec = task_sec_const.ToString
                Me.Label3.Text = "00:" + ssec
            Else
                min = Math.Floor(task_sec_const / 60)
                sec = task_sec_const - (min * 60)
                If min < 10 Then smin = "0" + min.ToString Else smin = min.ToString
                If sec < 10 Then ssec = "0" + sec.ToString Else ssec = sec.ToString
                Me.Label3.Text = smin + ":" + ssec
            End If
        Else
            Me.Label5.Text = "00:00"
        End If
        Me.Panel1.Visible = False
        Me.Panel3.Visible = True
        Me.Panel5.Visible = True
        Me.Panel7.Visible = True
        Me.Panel8.Visible = True
        test_inprogress = True
        viewing_mark = False
        testtime_is_elapsed = False
        next_task()
        Timer1.Start()
    End Sub

    Private Sub next_task()
        If tasks_left = 0 Then
            give_mark()
            Exit Sub
        End If
        Dim c_task_type As Integer
        Dim c_tasktype_fetched As Boolean = False
        Do While c_tasktype_fetched = False
            c_task_type = RndInt(0, 7)
            Select Case c_task_type
                Case 0
                    If SETTINGS.OPTIONS("use_russian_alphabet") = True Then
                        Dim rindex As Integer = RndInt(0, RUSSIAN_ALPHABET.Length - 1)
                        screen_clear()
                        Me.Label8.Text = "Введите символ:"
                        needed_keytype = 0
                        Dim r As Integer = RndInt(0, 1)
                        If r = 0 Then
                            Me.Label10.Text = RUSSIAN_ALPHABET(rindex).name
                            Me.Label9.Text = RUSSIAN_ALPHABET(rindex).symbol
                            KEY_NEEDED.name = RUSSIAN_ALPHABET(rindex).name
                            KEY_NEEDED.symbol = RUSSIAN_ALPHABET(rindex).symbol
                            KEY_NEEDED.using_shift = RUSSIAN_ALPHABET(rindex).using_shift
                        Else
                            Me.Label10.Text = "Строчная русская буква """ + RUSSIAN_ALPHABET(rindex).symbol.ToLower + """"
                            Me.Label9.Text = RUSSIAN_ALPHABET(rindex).symbol.ToLower
                            KEY_NEEDED.name = "Строчная русская буква """ + RUSSIAN_ALPHABET(rindex).symbol.ToLower + """"
                            KEY_NEEDED.symbol = RUSSIAN_ALPHABET(rindex).symbol.ToLower
                            KEY_NEEDED.using_shift = False
                        End If
                        KEY_NEEDED.keycode = RUSSIAN_ALPHABET(rindex).keycode
                        KEY_NEEDED.layout_needed = RUSSIAN_ALPHABET(rindex).layout_needed
                        KEY_NEEDED.using_ctrl = RUSSIAN_ALPHABET(rindex).using_ctrl
                        KEY_NEEDED.using_alt = RUSSIAN_ALPHABET(rindex).using_alt
                        Me.Label8.Visible = True
                        Me.Label10.Visible = True
                        Me.Label9.Visible = True
                        If task_sec_const > 0 Then task_sec = task_sec_const
                        c_tasktype_fetched = True
                    Else
                        Continue Do
                    End If
                Case 1
                    If SETTINGS.OPTIONS("use_english_alphabet") = True Then
                        Dim rindex As Integer = RndInt(0, ENGLISH_ALPHABET.Length - 1)
                        screen_clear()
                        Me.Label8.Text = "Введите символ:"
                        needed_keytype = 1
                        Dim r As Integer = RndInt(0, 1)
                        If r = 0 Then
                            Me.Label10.Text = ENGLISH_ALPHABET(rindex).name
                            Me.Label9.Text = ENGLISH_ALPHABET(rindex).symbol
                            KEY_NEEDED.name = ENGLISH_ALPHABET(rindex).name
                            KEY_NEEDED.symbol = ENGLISH_ALPHABET(rindex).symbol
                            KEY_NEEDED.using_shift = ENGLISH_ALPHABET(rindex).using_shift
                        Else
                            Me.Label10.Text = "Строчная английская буква """ + ENGLISH_ALPHABET(rindex).symbol.ToLower + """"
                            Me.Label9.Text = ENGLISH_ALPHABET(rindex).symbol.ToLower
                            KEY_NEEDED.name = "Строчная английская буква """ + ENGLISH_ALPHABET(rindex).symbol.ToLower + """"
                            KEY_NEEDED.symbol = ENGLISH_ALPHABET(rindex).symbol.ToLower
                            KEY_NEEDED.using_shift = False
                        End If
                        KEY_NEEDED.keycode = ENGLISH_ALPHABET(rindex).keycode
                        KEY_NEEDED.layout_needed = ENGLISH_ALPHABET(rindex).layout_needed
                        KEY_NEEDED.using_ctrl = ENGLISH_ALPHABET(rindex).using_ctrl
                        KEY_NEEDED.using_alt = ENGLISH_ALPHABET(rindex).using_alt
                        Me.Label8.Visible = True
                        Me.Label10.Visible = True
                        Me.Label9.Visible = True
                        If task_sec_const > 0 Then task_sec = task_sec_const
                        c_tasktype_fetched = True
                    Else
                        Continue Do
                    End If
                Case 2
                    If SETTINGS.OPTIONS("use_numeric_keys") = True Then
                        Dim rindex As Integer = RndInt(0, CYPHERS.Length - 1)
                        screen_clear()
                        Me.Label8.Text = "Введите символ:"
                        Me.Label10.Text = CYPHERS(rindex).name
                        Me.Label9.Text = CYPHERS(rindex).symbol
                        Me.Label8.Visible = True
                        Me.Label10.Visible = True
                        Me.Label9.Visible = True
                        needed_keytype = 2
                        KEY_NEEDED.keycode = CYPHERS(rindex).keycode
                        KEY_NEEDED.name = CYPHERS(rindex).name
                        KEY_NEEDED.symbol = CYPHERS(rindex).symbol
                        KEY_NEEDED.layout_needed = CYPHERS(rindex).layout_needed
                        KEY_NEEDED.using_shift = CYPHERS(rindex).using_shift
                        KEY_NEEDED.using_ctrl = CYPHERS(rindex).using_ctrl
                        KEY_NEEDED.using_alt = CYPHERS(rindex).using_alt
                        If task_sec_const > 0 Then task_sec = task_sec_const
                        c_tasktype_fetched = True
                    Else
                        Continue Do
                    End If
                Case 3
                    If SETTINGS.OPTIONS("use_cursor_control_keys") = True Then
                        Dim rindex As Integer = RndInt(0, CURSOR_CONTROL_KEYS.Length - 1)
                        screen_clear()
                        Me.Label8.Text = "Нажмите клавишу:"
                        Me.Label10.Text = "Клавиша " + CURSOR_CONTROL_KEYS(rindex).name
                        Me.Label11.Text = CURSOR_CONTROL_KEYS(rindex).symbol
                        Me.Label8.Visible = True
                        Me.Label10.Visible = True
                        Me.Label11.Visible = True
                        needed_keytype = 3
                        KEY_NEEDED.keycode = CURSOR_CONTROL_KEYS(rindex).keycode
                        KEY_NEEDED.name = "Клавиша " + CURSOR_CONTROL_KEYS(rindex).name
                        KEY_NEEDED.symbol = CURSOR_CONTROL_KEYS(rindex).symbol
                        KEY_NEEDED.layout_needed = CURSOR_CONTROL_KEYS(rindex).layout_needed
                        KEY_NEEDED.using_shift = CURSOR_CONTROL_KEYS(rindex).using_shift
                        KEY_NEEDED.using_ctrl = CURSOR_CONTROL_KEYS(rindex).using_ctrl
                        KEY_NEEDED.using_alt = CURSOR_CONTROL_KEYS(rindex).using_alt
                        If task_sec_const > 0 Then task_sec = task_sec_const
                        c_tasktype_fetched = True
                    Else
                        Continue Do
                    End If
                Case 4
                    If SETTINGS.OPTIONS("use_functional_keys") = True Then
                        Dim rindex As Integer = RndInt(0, FUNCTIONAL_KEYS.Length - 1)
                        screen_clear()
                        Me.Label8.Text = "Нажмите клавишу:"
                        Me.Label10.Text = FUNCTIONAL_KEYS(rindex).name
                        Me.Label11.Text = FUNCTIONAL_KEYS(rindex).symbol
                        Me.Label8.Visible = True
                        Me.Label10.Visible = True
                        Me.Label11.Visible = True
                        needed_keytype = 4
                        KEY_NEEDED.keycode = FUNCTIONAL_KEYS(rindex).keycode
                        KEY_NEEDED.name = FUNCTIONAL_KEYS(rindex).name
                        KEY_NEEDED.symbol = FUNCTIONAL_KEYS(rindex).symbol
                        KEY_NEEDED.layout_needed = FUNCTIONAL_KEYS(rindex).layout_needed
                        KEY_NEEDED.using_shift = FUNCTIONAL_KEYS(rindex).using_shift
                        KEY_NEEDED.using_ctrl = FUNCTIONAL_KEYS(rindex).using_ctrl
                        KEY_NEEDED.using_alt = FUNCTIONAL_KEYS(rindex).using_alt
                        If task_sec_const > 0 Then task_sec = task_sec_const
                        c_tasktype_fetched = True
                    Else
                        Continue Do
                    End If
                Case 5
                    If SETTINGS.OPTIONS("use_phrases") = True Then
                        Dim rindex As Integer = RndInt(0, PHRASES.Length - 1)
                        screen_clear()
                        Me.Label8.Text = "Введите фразу:"
                        Me.Label10.Text = "После ввода нажмите клавишу Enter"
                        Me.Label12.Text = PHRASES(rindex)
                        Me.Label8.Visible = True
                        Me.Label10.Visible = True
                        Me.Label12.Visible = True
                        Me.TextBox1.Visible = True
                        Me.TextBox1.Focus()
                        needed_keytype = 5
                        PHRASE_NEEDED = PHRASES(rindex)
                        If task_sec_const > 0 Then task_sec = phrasetask_sec_const
                        test_sec = test_sec + phrasetask_sec_const
                        c_tasktype_fetched = True
                    Else
                        Continue Do
                    End If
                Case 6
                    If SETTINGS.OPTIONS("use_misc_symbols") = True Then
                        Dim rindex As Integer = RndInt(0, MISC_SYMBOLS.Length - 1)
                        screen_clear()
                        Me.Label8.Text = "Введите символ:"
                        Me.Label10.Text = MISC_SYMBOLS(rindex).name
                        Me.Label9.Text = MISC_SYMBOLS(rindex).symbol
                        Me.Label8.Visible = True
                        Me.Label10.Visible = True
                        Me.Label9.Visible = True
                        needed_keytype = 6
                        KEY_NEEDED.keycode = MISC_SYMBOLS(rindex).keycode
                        KEY_NEEDED.name = MISC_SYMBOLS(rindex).name
                        KEY_NEEDED.symbol = MISC_SYMBOLS(rindex).symbol
                        KEY_NEEDED.layout_needed = MISC_SYMBOLS(rindex).layout_needed
                        KEY_NEEDED.using_shift = MISC_SYMBOLS(rindex).using_shift
                        KEY_NEEDED.using_ctrl = MISC_SYMBOLS(rindex).using_ctrl
                        KEY_NEEDED.using_alt = MISC_SYMBOLS(rindex).using_alt
                        If task_sec_const > 0 Then task_sec = task_sec_const
                        c_tasktype_fetched = True
                    Else
                        Continue Do
                    End If
                Case 7
                    If SETTINGS.OPTIONS("use_key_shortcuts") = True Then
                        Dim rindex As Integer = RndInt(0, KEY_SHORTCUTS.Length - 1)
                        screen_clear()
                        Me.Label8.Text = "Нажмите сочетание клавиш:"
                        Me.Label10.Text = "Сочетания клавиш вводятся на АНГЛИЙСКОЙ раскладке клавиатуры"
                        Dim sname As String = ""
                        If KEY_SHORTCUTS(rindex).using_shift = True Then sname += "Shift + "
                        If KEY_SHORTCUTS(rindex).using_ctrl = True Then sname += "Ctrl + "
                        If KEY_SHORTCUTS(rindex).using_alt = True Then sname += "Alt + "
                        sname += KEY_SHORTCUTS(rindex).symbol.ToString
                        Me.Label11.Text = sname
                        Me.Label8.Visible = True
                        Me.Label10.Visible = True
                        Me.Label11.Visible = True
                        needed_keytype = 7
                        KEY_NEEDED.using_shift = KEY_SHORTCUTS(rindex).using_shift
                        KEY_NEEDED.using_ctrl = KEY_SHORTCUTS(rindex).using_ctrl
                        KEY_NEEDED.using_alt = KEY_SHORTCUTS(rindex).using_alt
                        KEY_NEEDED.symbol = KEY_SHORTCUTS(rindex).symbol
                        KEY_NEEDED.keycode = KEY_SHORTCUTS(rindex).keycode
                        KEY_NEEDED.layout_needed = KEY_SHORTCUTS(rindex).layout_needed
                        KEY_NEEDED.name = KEY_SHORTCUTS(rindex).name
                        If task_sec_const > 0 Then task_sec = task_sec_const
                        c_tasktype_fetched = True
                    Else
                        Continue Do
                    End If
            End Select
        Loop
        Me.Label6.Text = "Осталось заданий " & tasks_left & " из " & task_number
    End Sub

    Private Sub screen_clear()
        Me.Label8.Visible = False
        Me.Label10.Visible = False
        Me.Label9.Visible = False
        Me.Label9.ForeColor = Color.White
        Me.Label11.Visible = False
        Me.Label11.ForeColor = Color.White
        Me.Label12.Visible = False
        Me.TextBox1.Visible = False
        Me.TextBox1.Text = ""
        Me.TextBox1.BackColor = Color.White
        Me.TextBox1.ForeColor = Drawing.Color.Black
    End Sub

    Private Function RndInt(ByVal lower As Integer, ByVal upper As Integer) As Integer
        Return CInt(Math.Floor((upper - lower + 1) * Rnd())) + lower
    End Function

    Private Sub colordelay(ByVal color As Integer, ByVal returnwhite As Boolean, ByVal delay As Integer)
        Me.KeyPreview = False
        Dim red, green, yellow As New System.Drawing.Color
        red = Drawing.Color.FromArgb(188, 15, 24)
        green = Drawing.Color.Green
        yellow = Drawing.Color.FromArgb(214, 204, 112)
        Dim colors(3) As Object
        colors = {red, green, yellow}
        If needed_keytype = 0 Or needed_keytype = 1 Or needed_keytype = 2 Or needed_keytype = 6 Then
            Me.Label9.ForeColor = colors(color)
            Me.Label9.Refresh()
        ElseIf needed_keytype = 3 Or needed_keytype = 4 Or needed_keytype = 7 Then
            Me.Label11.ForeColor = colors(color)
            Me.Label11.Refresh()
        ElseIf needed_keytype = 5 Then
            Me.TextBox1.BackColor = colors(color)
            Me.TextBox1.ForeColor = Drawing.Color.White
            Me.TextBox1.Refresh()
        End If
        System.Threading.Thread.Sleep(delay * 1000)
        If returnwhite = True Then
            If needed_keytype = 0 Or needed_keytype = 1 Or needed_keytype = 2 Or needed_keytype = 6 Then
                Me.Label9.ForeColor = Drawing.Color.White
                Me.Label9.Refresh()
            ElseIf needed_keytype = 3 Or needed_keytype = 4 Or needed_keytype = 7 Then
                Me.Label11.ForeColor = Drawing.Color.White
                Me.Label11.Refresh()
            ElseIf needed_keytype = 5 Then
                Me.TextBox1.BackColor = Drawing.Color.White
                Me.TextBox1.ForeColor = Drawing.Color.Black
                Me.TextBox1.Refresh()
            End If
        End If
        Me.KeyPreview = True
    End Sub

    Private Sub give_mark()
        test_inprogress = False
        Timer1.Stop()
        Me.Panel8.Visible = False
        Me.Panel5.Visible = False
        Me.Panel3.Visible = False
        Me.Panel7.Visible = False
        'Well, lets count right answers
        Dim tasks_done As Integer = 0
        Dim i As Integer = 0
        Dim tasks_passed As Integer = 0
        Dim tskp As Boolean
        SUMMARY.ListView1.Items.Clear()
        Do Until i = ANSWERS.Length
            If Not ANSWERS(i) Is Nothing Then
                tskp = False
                tasks_done = tasks_done + 1
                If Not ANSWERS(i).tsk_type = 5 Then
                    If ANSWERS(i).key_needed.keycode = ANSWERS(i).key_pressed.keycode And
                        ANSWERS(i).key_needed.using_shift = ANSWERS(i).key_pressed.using_shift And
                        ANSWERS(i).key_needed.using_ctrl = ANSWERS(i).key_pressed.using_ctrl And
                        ANSWERS(i).key_needed.using_alt = ANSWERS(i).key_pressed.using_alt And
                        (Not InStr(ANSWERS(i).key_needed.layout_needed, ANSWERS(i).key_pressed.layout_needed) = 0 Or ANSWERS(i).key_needed.layout_needed = "any") Then
                        tskp = True
                        tasks_passed = tasks_passed + 1
                    End If
                Else
                    If ANSWERS(i).phrase_needed = ANSWERS(i).phrase_entered Then
                        tskp = True
                        tasks_passed = tasks_passed + 1
                    End If
                End If
                Dim lvitem As New ListViewItem(i + 1)
                If Not ANSWERS(i).tsk_type = 5 Then
                    If Not ANSWERS(i).tsk_type = 7 Then lvitem.SubItems.Add(ANSWERS(i).key_needed.symbol) Else lvitem.SubItems.Add(getSymbolByKeycode(7, ANSWERS(i).key_needed.keycode, ANSWERS(i).key_needed.using_shift))
                    lvitem.SubItems.Add(getSymbolByKeycode(ANSWERS(i).tsk_type, ANSWERS(i).key_pressed.keycode, ANSWERS(i).key_pressed.using_shift))
                Else
                    lvitem.SubItems.Add(ANSWERS(i).phrase_needed)
                    lvitem.SubItems.Add(ANSWERS(i).phrase_entered)
                End If
                If tskp Then lvitem.SubItems.Add("+") Else lvitem.SubItems.Add("-")
                If ANSWERS(i).seconds_spent = 0 Then lvitem.SubItems.Add("N/A") Else lvitem.SubItems.Add((task_sec_const - ANSWERS(i).seconds_spent).ToString)
                SUMMARY.ListView1.Items.AddRange(New ListViewItem() {lvitem})
            End If
            i = i + 1
        Loop
        Dim proc As Double = Math.Floor((tasks_passed / task_number) * 100)
        Me.Label15.Text = "Выполнено заданий: " + tasks_done.ToString + " из предложенных " + task_number.ToString
        Me.Label16.Text = "Из них выполнено правильно: " + tasks_passed.ToString + " (" + proc.ToString + "%)"
        Dim tsp As Integer = SETTINGS.OPTIONS("test_time") - test_sec
        If tsp < 60 Then
            Me.Label17.Text = "Затрачено времени: " + tsp.ToString + " сек."
        Else
            Dim min, sec As Integer
            min = Math.Floor(tsp / 60)
            sec = tsp - (min * 60)
            Me.Label17.Text = "Затрачено времени: " + min.ToString + " мин. " + sec.ToString + " сек."
        End If
        If SETTINGS.OPTIONS("use_marks") = False Then
            Me.Label13.Text = "?"
            Me.Label18.Text = "Ты можешь посмотреть ошибки, нажав кнопку Сводка о работе или пройти тренировку еще раз."
            If testtime_is_elapsed = False Then Me.Label14.Text = "Работа завершена" Else Me.Label14.Text = "Время истекло :("
        Else
            Me.Button6.Enabled = False
            viewing_mark = True
            Me.Label18.Text = "Ты можешь посмотреть ошибки, нажав кнопку Сводка о работе или позвать учителя."
            If testtime_is_elapsed = False Then Me.Label14.Text = "Ваша оценка" Else Me.Label14.Text = "Время истекло :("
            Dim marked As Boolean = False
            If proc <= SETTINGS.OPTIONS("mark_five_persentage")(0) And proc >= SETTINGS.OPTIONS("mark_five_persentage")(1) Then
                Me.Label13.Text = "5"
                marked = True
            End If
            If proc <= SETTINGS.OPTIONS("mark_four_persentage")(0) And proc >= SETTINGS.OPTIONS("mark_four_persentage")(1) And marked = False Then
                Me.Label13.Text = "4"
                marked = True
            End If
            If proc <= SETTINGS.OPTIONS("mark_three_persentage")(0) And proc >= SETTINGS.OPTIONS("mark_three_persentage")(1) And marked = False Then
                Me.Label13.Text = "3"
                marked = True
            End If
            If marked = False Then
                Me.Label13.Text = "2"
            End If
        End If
        Me.Panel9.Visible = True
        Me.Button5.Focus()
    End Sub

    Private Function getSymbolByKeycode(ByVal tsk_type As Integer, ByVal keycode As Integer, ByVal using_shift As Boolean) As String
        Dim i As Integer = 0
        Dim symbol As String = ""
        Select Case tsk_type
            Case 0
                Do Until i = RUSSIAN_ALPHABET.Length
                    If RUSSIAN_ALPHABET(i).keycode = keycode Then
                        If using_shift = True Then
                            Return RUSSIAN_ALPHABET(i).symbol
                            Exit Function
                        Else
                            symbol = RUSSIAN_ALPHABET(i).symbol
                            Return symbol.ToLower
                            Exit Function
                        End If
                    End If
                    i = i + 1
                Loop
            Case 1
                Do Until i = ENGLISH_ALPHABET.Length
                    If ENGLISH_ALPHABET(i).keycode = keycode Then
                        If using_shift = True Then
                            Return ENGLISH_ALPHABET(i).symbol
                            Exit Function
                        Else
                            symbol = ENGLISH_ALPHABET(i).symbol
                            Return symbol.ToLower
                            Exit Function
                        End If
                    End If
                    i = i + 1
                Loop
            Case 2
                Do Until i = CYPHERS.Length
                    If CYPHERS(i).keycode = keycode Then
                        Return CYPHERS(i).symbol
                        Exit Function
                    End If
                    i = i + 1
                Loop
            Case 3
                Do Until i = CURSOR_CONTROL_KEYS.Length
                    If CURSOR_CONTROL_KEYS(i).keycode = keycode Then
                        Return CURSOR_CONTROL_KEYS(i).symbol
                        Exit Function
                    End If
                    i = i + 1
                Loop
            Case 4
                Do Until i = FUNCTIONAL_KEYS.Length
                    If FUNCTIONAL_KEYS(i).keycode = keycode Then
                        Return FUNCTIONAL_KEYS(i).symbol
                        Exit Function
                    End If
                    i = i + 1
                Loop
            Case 6
                Do Until i = MISC_SYMBOLS.Length
                    If MISC_SYMBOLS(i).keycode = keycode Then
                        Return MISC_SYMBOLS(i).symbol
                        Exit Function
                    End If
                    i = i + 1
                Loop
            Case 7
                Do Until i = KEY_SHORTCUTS.Length
                    If KEY_SHORTCUTS(i).keycode = keycode Then
                        Dim sname As String = ""
                        If KEY_SHORTCUTS(i).using_shift = True Then sname += "Shift + "
                        If KEY_SHORTCUTS(i).using_ctrl = True Then sname += "Ctrl + "
                        If KEY_SHORTCUTS(i).using_alt = True Then sname += "Alt + "
                        sname += KEY_SHORTCUTS(i).symbol.ToString
                        Return sname
                        Exit Function
                    End If
                    i = i + 1
                Loop
        End Select
    End Function

    Private Sub SymbolCollections_Init()
        'Define symbol collections
        Dim l1 As New KEY
        l1.name = "Заглавная русская буква ""А"""
        l1.symbol = "А"
        l1.layout_needed = "Русская"
        l1.keycode = 70
        l1.using_shift = True
        l1.using_ctrl = False
        l1.using_alt = False

        Dim l2 As New KEY
        l2.name = "Заглавная русская буква ""Б"""
        l2.symbol = "Б"
        l2.layout_needed = "Русская"
        l2.keycode = 188
        l2.using_shift = True
        l2.using_ctrl = False
        l2.using_alt = False

        Dim l3 As New KEY
        l3.name = "Заглавная русская буква ""В"""
        l3.symbol = "В"
        l3.layout_needed = "Русская"
        l3.keycode = 68
        l3.using_shift = True
        l3.using_ctrl = False
        l3.using_alt = False

        Dim l4 As New KEY
        l4.name = "Заглавная русская буква ""Г"""
        l4.symbol = "Г"
        l4.layout_needed = "Русская"
        l4.keycode = 85
        l4.using_shift = True
        l4.using_ctrl = False
        l4.using_alt = False

        Dim l5 As New KEY
        l5.name = "Заглавная русская буква ""Д"""
        l5.symbol = "Д"
        l5.layout_needed = "Русская"
        l5.keycode = 76
        l5.using_shift = True
        l5.using_ctrl = False
        l5.using_alt = False

        Dim l6 As New KEY
        l6.name = "Заглавная русская буква ""Е"""
        l6.symbol = "Е"
        l6.layout_needed = "Русская"
        l6.keycode = 84
        l6.using_shift = True
        l6.using_ctrl = False
        l6.using_alt = False

        Dim l7 As New KEY
        l7.name = "Заглавная русская буква ""Ё"""
        l7.symbol = "Ё"
        l7.layout_needed = "Русская"
        l7.keycode = 192
        l7.using_shift = True
        l7.using_ctrl = False
        l7.using_alt = False

        Dim l8 As New KEY
        l8.name = "Заглавная русская буква ""Ж"""
        l8.symbol = "Ж"
        l8.layout_needed = "Русская"
        l8.keycode = 186
        l8.using_shift = True
        l8.using_ctrl = False
        l8.using_alt = False

        Dim l9 As New KEY
        l9.name = "Заглавная русская буква ""З"""
        l9.symbol = "З"
        l9.layout_needed = "Русская"
        l9.keycode = 80
        l9.using_shift = True
        l9.using_ctrl = False
        l9.using_alt = False

        Dim l10 As New KEY
        l10.name = "Заглавная русская буква ""И"""
        l10.symbol = "И"
        l10.layout_needed = "Русская"
        l10.keycode = 66
        l10.using_shift = True
        l10.using_ctrl = False
        l10.using_alt = False

        Dim l11 As New KEY
        l11.name = "Заглавная русская буква ""К"""
        l11.symbol = "К"
        l11.layout_needed = "Русская"
        l11.keycode = 82
        l11.using_shift = True
        l11.using_ctrl = False
        l11.using_alt = False

        Dim l12 As New KEY
        l12.name = "Заглавная русская буква ""Л"""
        l12.symbol = "Л"
        l12.layout_needed = "Русская"
        l12.keycode = 75
        l12.using_shift = True
        l12.using_ctrl = False
        l12.using_alt = False

        Dim l13 As New KEY
        l13.name = "Заглавная русская буква ""М"""
        l13.symbol = "М"
        l13.layout_needed = "Русская"
        l13.keycode = 86
        l13.using_shift = True
        l13.using_ctrl = False
        l13.using_alt = False

        Dim l14 As New KEY
        l14.name = "Заглавная русская буква ""Н"""
        l14.symbol = "Н"
        l14.layout_needed = "Русская"
        l14.keycode = 89
        l14.using_shift = True
        l14.using_ctrl = False
        l14.using_alt = False

        Dim l15 As New KEY
        l15.name = "Заглавная русская буква ""О"""
        l15.symbol = "О"
        l15.layout_needed = "Русская"
        l15.keycode = 74
        l15.using_shift = True
        l15.using_ctrl = False
        l15.using_alt = False

        Dim l16 As New KEY
        l16.name = "Заглавная русская буква ""П"""
        l16.symbol = "П"
        l16.layout_needed = "Русская"
        l16.keycode = 71
        l16.using_shift = True
        l16.using_ctrl = False
        l16.using_alt = False

        Dim l17 As New KEY
        l17.name = "Заглавная русская буква ""Р"""
        l17.symbol = "Р"
        l17.layout_needed = "Русская"
        l17.keycode = 72
        l17.using_shift = True
        l17.using_ctrl = False
        l17.using_alt = False

        Dim l18 As New KEY
        l18.name = "Заглавная русская буква ""С"""
        l18.symbol = "С"
        l18.layout_needed = "Русская"
        l18.keycode = 67
        l18.using_shift = True
        l18.using_ctrl = False
        l18.using_alt = False

        Dim l19 As New KEY
        l19.name = "Заглавная русская буква ""Т"""
        l19.symbol = "Т"
        l19.layout_needed = "Русская"
        l19.keycode = 78
        l19.using_shift = True
        l19.using_ctrl = False
        l19.using_alt = False

        Dim l20 As New KEY
        l20.name = "Заглавная русская буква ""У"""
        l20.symbol = "У"
        l20.layout_needed = "Русская"
        l20.keycode = 69
        l20.using_shift = True
        l20.using_ctrl = False
        l20.using_alt = False

        Dim l21 As New KEY
        l21.name = "Заглавная русская буква ""Ф"""
        l21.symbol = "Ф"
        l21.layout_needed = "Русская"
        l21.keycode = 65
        l21.using_shift = True
        l21.using_ctrl = False
        l21.using_alt = False

        Dim l22 As New KEY
        l22.name = "Заглавная русская буква ""Х"""
        l22.symbol = "Х"
        l22.layout_needed = "Русская"
        l22.keycode = 219
        l22.using_shift = True
        l22.using_ctrl = False
        l22.using_alt = False

        Dim l23 As New KEY
        l23.name = "Заглавная русская буква ""Ц"""
        l23.symbol = "Ц"
        l23.layout_needed = "Русская"
        l23.keycode = 87
        l23.using_shift = True
        l23.using_ctrl = False
        l23.using_alt = False

        Dim l24 As New KEY
        l24.name = "Заглавная русская буква ""Ч"""
        l24.symbol = "Ч"
        l24.layout_needed = "Русская"
        l24.keycode = 88
        l24.using_shift = True
        l24.using_ctrl = False
        l24.using_alt = False

        Dim l25 As New KEY
        l25.name = "Заглавная русская буква ""Ш"""
        l25.symbol = "Ш"
        l25.layout_needed = "Русская"
        l25.keycode = 73
        l25.using_shift = True
        l25.using_ctrl = False
        l25.using_alt = False

        Dim l26 As New KEY
        l26.name = "Заглавная русская буква ""Щ"""
        l26.symbol = "Щ"
        l26.layout_needed = "Русская"
        l26.keycode = 79
        l26.using_shift = True
        l26.using_ctrl = False
        l26.using_alt = False

        Dim l27 As New KEY
        l27.name = "Заглавная русская буква ""Ъ"""
        l27.symbol = "Ъ"
        l27.layout_needed = "Русская"
        l27.keycode = 221
        l27.using_shift = True
        l27.using_ctrl = False
        l27.using_alt = False

        Dim l28 As New KEY
        l28.name = "Заглавная русская буква ""Ы"""
        l28.symbol = "Ы"
        l28.layout_needed = "Русская"
        l28.keycode = 83
        l28.using_shift = True
        l28.using_ctrl = False
        l28.using_alt = False

        Dim l29 As New KEY
        l29.name = "Заглавная русская буква ""Ь"""
        l29.symbol = "Ь"
        l29.layout_needed = "Русская"
        l29.keycode = 77
        l29.using_shift = True
        l29.using_ctrl = False
        l29.using_alt = False

        Dim l30 As New KEY
        l30.name = "Заглавная русская буква ""Э"""
        l30.symbol = "Э"
        l30.layout_needed = "Русская"
        l30.keycode = 222
        l30.using_shift = True
        l30.using_ctrl = False
        l30.using_alt = False

        Dim l31 As New KEY
        l31.name = "Заглавная русская буква ""Ю"""
        l31.symbol = "Ю"
        l31.layout_needed = "Русская"
        l31.keycode = 190
        l31.using_shift = True
        l31.using_ctrl = False
        l31.using_alt = False

        Dim l32 As New KEY
        l32.name = "Заглавная русская буква ""Я"""
        l32.symbol = "Я"
        l32.layout_needed = "Русская"
        l32.keycode = 90
        l32.using_shift = True
        l32.using_ctrl = False
        l32.using_alt = False

        Dim l33 As New KEY
        l33.name = "Заглавная русская буква ""Й"""
        l33.symbol = "Й"
        l33.layout_needed = "Русская"
        l33.keycode = 81
        l33.using_shift = True
        l33.using_ctrl = False
        l33.using_alt = False

        RUSSIAN_ALPHABET = {l1, l2, l3, l4, l5, l6, l7, l8, l9, l10, l11, l12, l13, l14, l15, l16, l17, l18, l19, l20, l21, l22, l23, l24, l25, l26, l27, l28, l29, l30, l31, l32, l33}

        l1.name = "Заглавная английская буква ""A"""
        l1.symbol = "A"
        l1.layout_needed = "Британская|США"
        l1.keycode = 65
        l1.using_shift = True
        l1.using_ctrl = False
        l1.using_alt = False

        l2.name = "Заглавная английская буква ""B"""
        l2.symbol = "B"
        l2.layout_needed = "Британская|США"
        l2.keycode = 66
        l2.using_shift = True
        l2.using_ctrl = False
        l2.using_alt = False

        l3.name = "Заглавная английская буква ""C"""
        l3.symbol = "C"
        l3.layout_needed = "Британская|США"
        l3.keycode = 67
        l3.using_shift = True
        l3.using_ctrl = False
        l3.using_alt = False

        l4.name = "Заглавная английская буква ""D"""
        l4.symbol = "D"
        l4.layout_needed = "Британская|США"
        l4.keycode = 68
        l4.using_shift = True
        l4.using_ctrl = False
        l4.using_alt = False

        l5.name = "Заглавная английская буква ""E"""
        l5.symbol = "E"
        l5.layout_needed = "Британская|США"
        l5.keycode = 69
        l5.using_shift = True
        l5.using_ctrl = False
        l5.using_alt = False

        l6.name = "Заглавная английская буква ""F"""
        l6.symbol = "F"
        l6.layout_needed = "Британская|США"
        l6.keycode = 70
        l6.using_shift = True
        l6.using_ctrl = False
        l6.using_alt = False

        l7.name = "Заглавная английская буква ""G"""
        l7.symbol = "G"
        l7.layout_needed = "Британская|США"
        l7.keycode = 71
        l7.using_shift = True
        l7.using_ctrl = False
        l7.using_alt = False

        l8.name = "Заглавная английская буква ""H"""
        l8.symbol = "H"
        l8.layout_needed = "Британская|США"
        l8.keycode = 72
        l8.using_shift = True
        l8.using_ctrl = False
        l8.using_alt = False

        l9.name = "Заглавная английская буква ""I"""
        l9.symbol = "I"
        l9.layout_needed = "Британская|США"
        l9.keycode = 73
        l9.using_shift = True
        l9.using_ctrl = False
        l9.using_alt = False

        l10.name = "Заглавная английская буква ""J"""
        l10.symbol = "J"
        l10.layout_needed = "Британская|США"
        l10.keycode = 74
        l10.using_shift = True
        l10.using_ctrl = False
        l10.using_alt = False

        l11.name = "Заглавная английская буква ""K"""
        l11.symbol = "K"
        l11.layout_needed = "Британская|США"
        l11.keycode = 75
        l11.using_shift = True
        l11.using_ctrl = False
        l11.using_alt = False

        l12.name = "Заглавная английская буква ""L"""
        l12.symbol = "L"
        l12.layout_needed = "Британская|США"
        l12.keycode = 76
        l12.using_shift = True
        l12.using_ctrl = False
        l12.using_alt = False

        l13.name = "Заглавная английская буква ""M"""
        l13.symbol = "M"
        l13.layout_needed = "Британская|США"
        l13.keycode = 77
        l13.using_shift = True
        l13.using_ctrl = False
        l13.using_alt = False

        l14.name = "Заглавная английская буква ""N"""
        l14.symbol = "N"
        l14.layout_needed = "Британская|США"
        l14.keycode = 78
        l14.using_shift = True
        l14.using_ctrl = False
        l14.using_alt = False

        l15.name = "Заглавная английская буква ""O"""
        l15.symbol = "O"
        l15.layout_needed = "Британская|США"
        l15.keycode = 79
        l15.using_shift = True
        l15.using_ctrl = False
        l15.using_alt = False

        l16.name = "Заглавная английская буква ""P"""
        l16.symbol = "P"
        l16.layout_needed = "Британская|США"
        l16.keycode = 80
        l16.using_shift = True
        l16.using_ctrl = False
        l16.using_alt = False

        l17.name = "Заглавная английская буква ""Q"""
        l17.symbol = "Q"
        l17.layout_needed = "Британская|США"
        l17.keycode = 81
        l17.using_shift = True
        l17.using_ctrl = False
        l17.using_alt = False

        l18.name = "Заглавная английская буква ""R"""
        l18.symbol = "R"
        l18.layout_needed = "Британская|США"
        l18.keycode = 82
        l18.using_shift = True
        l18.using_ctrl = False
        l18.using_alt = False

        l19.name = "Заглавная английская буква ""S"""
        l19.symbol = "S"
        l19.layout_needed = "Британская|США"
        l19.keycode = 83
        l19.using_shift = True
        l19.using_ctrl = False
        l19.using_alt = False

        l20.name = "Заглавная английская буква ""T"""
        l20.symbol = "T"
        l20.layout_needed = "Британская|США"
        l20.keycode = 84
        l20.using_shift = True
        l20.using_ctrl = False
        l20.using_alt = False

        l21.name = "Заглавная английская буква ""U"""
        l21.symbol = "U"
        l21.layout_needed = "Британская|США"
        l21.keycode = 85
        l21.using_shift = True
        l21.using_ctrl = False
        l21.using_alt = False

        l22.name = "Заглавная английская буква ""V"""
        l22.symbol = "V"
        l22.layout_needed = "Британская|США"
        l22.keycode = 86
        l22.using_shift = True
        l22.using_ctrl = False
        l22.using_alt = False

        l23.name = "Заглавная английская буква ""W"""
        l23.symbol = "W"
        l23.layout_needed = "Британская|США"
        l23.keycode = 87
        l23.using_shift = True
        l23.using_ctrl = False
        l23.using_alt = False

        l24.name = "Заглавная английская буква ""X"""
        l24.symbol = "X"
        l24.layout_needed = "Британская|США"
        l24.keycode = 88
        l24.using_shift = True
        l24.using_ctrl = False
        l24.using_alt = False

        l25.name = "Заглавная английская буква ""Y"""
        l25.symbol = "Y"
        l25.layout_needed = "Британская|США"
        l25.keycode = 89
        l25.using_shift = True
        l25.using_ctrl = False
        l25.using_alt = False

        l26.name = "Заглавная английская буква ""Z"""
        l26.symbol = "Z"
        l26.layout_needed = "Британская|США"
        l26.keycode = 90
        l26.using_shift = True
        l26.using_ctrl = False
        l26.using_alt = False

        ENGLISH_ALPHABET = {l2, l3, l4, l5, l6, l7, l8, l9, l10, l11, l12, l13, l14, l15, l16, l17, l18, l19, l20, l21, l22, l23, l24, l25, l26}

        l1.name = "Цифра 1"
        l1.symbol = "1"
        l1.layout_needed = "any"
        l1.keycode = 49
        l1.using_shift = False
        l1.using_ctrl = False
        l1.using_alt = False

        l2.name = "Цифра 2"
        l2.symbol = "2"
        l2.layout_needed = "any"
        l2.keycode = 50
        l2.using_shift = False
        l2.using_ctrl = False
        l2.using_alt = False

        l3.name = "Цифра 3"
        l3.symbol = "3"
        l3.layout_needed = "any"
        l3.keycode = 51
        l3.using_shift = False
        l3.using_ctrl = False
        l3.using_alt = False

        l4.name = "Цифра 4"
        l4.symbol = "4"
        l4.layout_needed = "any"
        l4.keycode = 52
        l4.using_shift = False
        l4.using_ctrl = False
        l4.using_alt = False

        l5.name = "Цифра 5"
        l5.symbol = "5"
        l5.layout_needed = "any"
        l5.keycode = 53
        l5.using_shift = False
        l5.using_ctrl = False
        l5.using_alt = False

        l6.name = "Цифра 6"
        l6.symbol = "6"
        l6.layout_needed = "any"
        l6.keycode = 54
        l6.using_shift = False
        l6.using_ctrl = False
        l6.using_alt = False

        l7.name = "Цифра 7"
        l7.symbol = "7"
        l7.layout_needed = "any"
        l7.keycode = 55
        l7.using_shift = False
        l7.using_ctrl = False
        l7.using_alt = False

        l8.name = "Цифра 8"
        l8.symbol = "8"
        l8.layout_needed = "any"
        l8.keycode = 56
        l8.using_shift = False
        l8.using_ctrl = False
        l8.using_alt = False

        l9.name = "Цифра 9"
        l9.symbol = "9"
        l9.layout_needed = "any"
        l9.keycode = 57
        l9.using_shift = False
        l9.using_ctrl = False
        l9.using_alt = False

        l10.name = "Цифра 0"
        l10.symbol = "0"
        l10.layout_needed = "any"
        l10.keycode = 48
        l10.using_shift = False
        l10.using_ctrl = False
        l10.using_alt = False

        CYPHERS = {l1, l2, l3, l4, l5, l6, l7, l8, l9, l10}

        l1.name = "Восклицательный знак"
        l1.symbol = "!"
        l1.layout_needed = "any"
        l1.keycode = 49
        l1.using_shift = True
        l1.using_ctrl = False
        l1.using_alt = False

        l2.name = "Знак порядкового номера"
        l2.symbol = "№"
        l2.layout_needed = "Русская"
        l2.keycode = 51
        l2.using_shift = True
        l2.using_ctrl = False
        l2.using_alt = False

        l3.name = "Точка с запятой"
        l3.symbol = ";"
        l3.layout_needed = "Русская"
        l3.keycode = 52
        l3.using_shift = True
        l3.using_ctrl = False
        l3.using_alt = False

        l4.name = "Знак процентов"
        l4.symbol = "%"
        l4.layout_needed = "Русская"
        l4.keycode = 53
        l4.using_shift = True
        l4.using_ctrl = False
        l4.using_alt = False

        l5.name = "Двоеточие"
        l5.symbol = ":"
        l5.layout_needed = "Русская"
        l5.keycode = 54
        l5.using_shift = True
        l5.using_ctrl = False
        l5.using_alt = False

        l6.name = "Вопросительный знак"
        l6.symbol = "?"
        l6.layout_needed = "Русская"
        l6.keycode = 55
        l6.using_shift = True
        l6.using_ctrl = False
        l6.using_alt = False

        l7.name = "Звездочка"
        l7.symbol = "*"
        l7.layout_needed = "any"
        l7.keycode = 56
        l7.using_shift = True
        l7.using_ctrl = False
        l7.using_alt = False

        l8.name = "Открывающаяся круглая скобка"
        l8.symbol = "("
        l8.layout_needed = "any"
        l8.keycode = 57
        l8.using_shift = True
        l8.using_ctrl = False
        l8.using_alt = False

        l9.name = "Закрывающаяся круглая скобка"
        l9.symbol = ")"
        l9.layout_needed = "any"
        l9.keycode = 48
        l9.using_shift = True
        l9.using_ctrl = False
        l9.using_alt = False

        l10.name = "Знак сложения"
        l10.symbol = "+"
        l10.layout_needed = "any"
        l10.keycode = 187
        l10.using_shift = True
        l10.using_ctrl = False
        l10.using_alt = False

        l11.name = "Знак равенства"
        l11.symbol = "="
        l11.layout_needed = "any"
        l11.keycode = 187
        l11.using_shift = False
        l11.using_ctrl = False
        l11.using_alt = False

        l12.name = "Открывающаяся квадратная скобка"
        l12.symbol = "["
        l12.layout_needed = "Британская|США"
        l12.keycode = 219
        l12.using_shift = False
        l12.using_ctrl = False
        l12.using_alt = False

        l13.name = "Закрывающаяся квадратная скобка"
        l13.symbol = "]"
        l13.layout_needed = "Британская|США"
        l13.keycode = 221
        l13.using_shift = False
        l13.using_ctrl = False
        l13.using_alt = False

        l14.name = "Открывающаяся фигурная скобка"
        l14.symbol = "{"
        l14.layout_needed = "Британская|США"
        l14.keycode = 219
        l14.using_shift = True
        l14.using_ctrl = False
        l14.using_alt = False

        l15.name = "Закрывающаяся фигурная скобка"
        l15.symbol = "}"
        l15.layout_needed = "Британская|США"
        l15.keycode = 221
        l15.using_shift = True
        l15.using_ctrl = False
        l15.using_alt = False

        l16.name = "Прямой слеш"
        l16.symbol = "/"
        l16.layout_needed = "Русская"
        l16.keycode = 220
        l16.using_shift = True
        l16.using_ctrl = False
        l16.using_alt = False

        l17.name = "Обратный слеш"
        l17.symbol = "\"
        l17.layout_needed = "Русская"
        l17.keycode = 220
        l17.using_shift = False
        l17.using_ctrl = False
        l17.using_alt = False

        l18.name = "Точка"
        l18.symbol = "."
        l18.layout_needed = "Русская"
        l18.keycode = 191
        l18.using_shift = False
        l18.using_ctrl = False
        l18.using_alt = False

        l19.name = "Запятая"
        l19.symbol = ","
        l19.layout_needed = "Русская"
        l19.keycode = 191
        l19.using_shift = True
        l19.using_ctrl = False
        l19.using_alt = False

        l20.name = "Знак вычитания"
        l20.symbol = "-"
        l20.layout_needed = "any"
        l20.keycode = 189
        l20.using_shift = False
        l20.using_ctrl = False
        l20.using_alt = False

        MISC_SYMBOLS = {l1, l2, l3, l4, l5, l6, l7, l8, l9, l10, l11, l12, l13, l14, l15, l16, l17, l18, l19, l20}

        Dim key_f1 As New KEY
        key_f1.symbol = "F1"
        key_f1.keycode = 112
        key_f1.name = "Функциональная клавиша F1"
        key_f1.layout_needed = "any"
        key_f1.using_shift = False
        key_f1.using_ctrl = False
        key_f1.using_alt = False

        Dim key_f2 As New KEY
        key_f2.symbol = "F2"
        key_f2.keycode = 113
        key_f2.name = "Функциональная клавиша F2"
        key_f2.layout_needed = "any"
        key_f2.using_shift = False
        key_f2.using_ctrl = False
        key_f2.using_alt = False

        Dim key_f3 As New KEY
        key_f3.symbol = "F3"
        key_f3.keycode = 114
        key_f3.name = "Функциональная клавиша F3"
        key_f3.layout_needed = "any"
        key_f3.using_shift = False
        key_f3.using_ctrl = False
        key_f3.using_alt = False

        Dim key_f4 As New KEY
        key_f4.symbol = "F4"
        key_f4.keycode = 115
        key_f4.name = "Функциональная клавиша F4"
        key_f4.layout_needed = "any"
        key_f4.using_shift = False
        key_f4.using_ctrl = False
        key_f4.using_alt = False

        Dim key_f5 As New KEY
        key_f5.symbol = "F5"
        key_f5.keycode = 116
        key_f5.name = "Функциональная клавиша F5"
        key_f5.layout_needed = "any"
        key_f5.using_shift = False
        key_f5.using_ctrl = False
        key_f5.using_alt = False

        Dim key_f6 As New KEY
        key_f6.symbol = "F6"
        key_f6.keycode = 117
        key_f6.name = "Функциональная клавиша F6"
        key_f6.layout_needed = "any"
        key_f6.using_shift = False
        key_f6.using_ctrl = False
        key_f6.using_alt = False

        Dim key_f7 As New KEY
        key_f7.symbol = "F7"
        key_f7.keycode = 118
        key_f7.name = "Функциональная клавиша F7"
        key_f7.layout_needed = "any"
        key_f7.using_shift = False
        key_f7.using_ctrl = False
        key_f7.using_alt = False

        Dim key_f8 As New KEY
        key_f8.symbol = "F8"
        key_f8.keycode = 119
        key_f8.name = "Функциональная клавиша F8"
        key_f8.layout_needed = "any"
        key_f8.using_shift = False
        key_f8.using_ctrl = False
        key_f8.using_alt = False

        Dim key_f9 As New KEY
        key_f9.symbol = "F9"
        key_f9.keycode = 120
        key_f9.name = "Функциональная клавиша F9"
        key_f9.layout_needed = "any"
        key_f9.using_shift = False
        key_f9.using_ctrl = False
        key_f9.using_alt = False

        Dim key_f10 As New KEY
        key_f10.symbol = "F10"
        key_f10.keycode = 121
        key_f10.name = "Функциональная клавиша F10"
        key_f10.layout_needed = "any"
        key_f10.using_shift = False
        key_f10.using_ctrl = False
        key_f10.using_alt = False

        Dim key_f11 As New KEY
        key_f11.symbol = "F11"
        key_f11.keycode = 122
        key_f11.name = "Функциональная клавиша F11"
        key_f11.layout_needed = "any"
        key_f11.using_shift = False
        key_f11.using_ctrl = False
        key_f11.using_alt = False

        Dim key_f12 As New KEY
        key_f12.symbol = "F12"
        key_f12.keycode = 123
        key_f12.name = "Функциональная клавиша F12"
        key_f12.layout_needed = "any"
        key_f12.using_shift = False
        key_f12.using_ctrl = False
        key_f12.using_alt = False

        FUNCTIONAL_KEYS = {key_f1, key_f2, key_f3, key_f4, key_f5, key_f6, key_f7, key_f8, key_f9, key_f10, key_f11, key_f12}

        Dim key_insert As New KEY
        key_insert.symbol = "Insert"
        key_insert.keycode = 45
        key_insert.name = "Insert"
        key_insert.layout_needed = "any"
        key_insert.using_shift = False
        key_insert.using_ctrl = False
        key_insert.using_alt = False

        Dim key_home As New KEY
        key_home.symbol = "Home"
        key_home.keycode = 36
        key_home.name = "Home"
        key_home.layout_needed = "any"
        key_home.using_shift = False
        key_home.using_ctrl = False
        key_home.using_alt = False

        Dim key_pageup As New KEY
        key_pageup.symbol = "PageUp"
        key_pageup.keycode = 33
        key_pageup.name = "Page Up"
        key_pageup.layout_needed = "any"
        key_pageup.using_shift = False
        key_pageup.using_ctrl = False
        key_pageup.using_alt = False

        Dim key_delete As New KEY
        key_delete.symbol = "Delete"
        key_delete.keycode = 46
        key_delete.name = "Delete"
        key_delete.layout_needed = "any"
        key_delete.using_shift = False
        key_delete.using_ctrl = False
        key_delete.using_alt = False

        Dim key_end As New KEY
        key_end.symbol = "End"
        key_end.keycode = 35
        key_end.name = "End"
        key_end.layout_needed = "any"
        key_end.using_shift = False
        key_end.using_ctrl = False
        key_end.using_alt = False

        Dim key_pagedown As New KEY
        key_pagedown.symbol = "PageDown"
        key_pagedown.keycode = 34
        key_pagedown.name = "Page Down"
        key_pagedown.layout_needed = "any"
        key_pagedown.using_shift = False
        key_pagedown.using_ctrl = False
        key_pagedown.using_alt = False

        Dim key_backspace As New KEY
        key_backspace.symbol = "Backspace"
        key_backspace.keycode = 8
        key_backspace.name = "Backspace"
        key_backspace.layout_needed = "any"
        key_backspace.using_shift = False
        key_backspace.using_ctrl = False
        key_backspace.using_alt = False

        Dim key_esc As New KEY
        key_esc.symbol = "Esc"
        key_esc.keycode = 27
        key_esc.name = "Escape"
        key_esc.layout_needed = "any"
        key_esc.using_shift = False
        key_esc.using_ctrl = False
        key_esc.using_alt = False

        Dim key_numlock As New KEY
        key_numlock.symbol = "NumLock"
        key_numlock.keycode = 144
        key_numlock.name = "Num Lock"
        key_numlock.layout_needed = "any"
        key_numlock.using_shift = False
        key_numlock.using_ctrl = False
        key_numlock.using_alt = False

        Dim key_rightarrow As New KEY
        key_rightarrow.symbol = "Стрелка вправо"
        key_rightarrow.keycode = 39
        key_rightarrow.name = "Стрелка вправо"
        key_rightarrow.layout_needed = "any"
        key_rightarrow.using_shift = False
        key_rightarrow.using_ctrl = False
        key_rightarrow.using_alt = False

        Dim key_leftarrow As New KEY
        key_leftarrow.symbol = "Стрелка влево"
        key_leftarrow.keycode = 37
        key_leftarrow.name = "Стрелка влево"
        key_leftarrow.layout_needed = "any"
        key_leftarrow.using_shift = False
        key_leftarrow.using_ctrl = False
        key_leftarrow.using_alt = False

        Dim key_uparrow As New KEY
        key_uparrow.symbol = "Стрелка вверх"
        key_uparrow.keycode = 38
        key_uparrow.name = "Стрелка вверх"
        key_uparrow.layout_needed = "any"
        key_uparrow.using_shift = False
        key_uparrow.using_ctrl = False
        key_uparrow.using_alt = False

        Dim key_downarrow As New KEY
        key_downarrow.symbol = "Стрелка вниз"
        key_downarrow.keycode = 40
        key_downarrow.name = "Стрелка вниз"
        key_downarrow.layout_needed = "any"
        key_downarrow.using_shift = False
        key_downarrow.using_ctrl = False
        key_downarrow.using_alt = False

        Dim key_enter As New KEY
        key_enter.symbol = "Enter"
        key_enter.keycode = 13
        key_enter.name = "Ввод"
        key_enter.layout_needed = "any"
        key_enter.using_shift = False
        key_enter.using_ctrl = False
        key_enter.using_alt = False

        Dim key_tab As New KEY
        key_tab.symbol = "TAB"
        key_tab.keycode = 9
        key_tab.name = "TAB"
        key_tab.layout_needed = "any"
        key_tab.using_shift = False
        key_tab.using_ctrl = False
        key_tab.using_alt = False

        Dim key_space As New KEY
        key_space.name = "Пробел"
        key_space.symbol = "Пробел"
        key_space.layout_needed = "any"
        key_space.keycode = 32
        key_space.using_shift = False
        key_space.using_ctrl = False
        key_space.using_alt = False

        CURSOR_CONTROL_KEYS = {key_insert, key_home, key_pageup, key_delete, key_end, key_pagedown, key_backspace, key_esc, key_numlock, key_downarrow, key_leftarrow, key_rightarrow, key_uparrow, key_enter, key_tab, key_space}

        PHRASES = {
            "А где щи, там и ищи.",
            "А дело бывало - и коза волка съедала.",
            "А как худ князь, так и в грязь.",
            "А когда досуг-то будет? - А когда нас не будет",
            "А кто слыхал, чтоб медведь летал?",
            "Барская милость - что кисельная сытость.",
            "Барская просьба - строгий приказ.",
            "Барская хворь - мужицкое здоровье.",
            "Без прикраски и слово не баско.",
            "Без работы и печь холодна.",
            "Без хвоста и пичужка не красна.",
            "Без хлеба да без каши ни во что и труды наши.",
            "Без хлеба и ситник в честь.",
            "Без хлеба и у воды худо жить.",
            "Без хлеба куска везде тоска.",
            "В мёртвые уста кусок не пройдёт, а живой как-нибудь проглотит.",
            "В мире, что в омуте: ни дна, ни покрышки.",
            "В огороде бузина, а в Киеве дядька.",
            "В одну руку всего не загребёшь.",
            "В озере два чёрта не живут.",
            "Гладок, мягок, да на вкус гадок.",
            "Глаза - как плошки, а не видят ни крошки.",
            "Глаза боятся, а руки делают.",
            "Глаза с поволокой, роток с позевотой.",
            "Глазам стыдно, а душа радуется.",
            "A bad beginning makes a bad ending.",
            "A bad workman blames his tools",
            "A bargain is a bargain.",
            "A broken friendship may be soldered, but will never be sound.",
            "A cat in gloves catches no mice",
            "Between two Chairs",
            "Beware of Greeks bearing gifts",
            "Birds of a feather flock together",
            "Blood is thicker than water",
            "Boys will be boys",
            "Don't cross the bridge till you come to it",
            "Don't keep a dog and bark yourself",
            "Don't rock the boat",
            "Don't look a gift horse in the mouth",
            "Don't put all your eggs in one basket",
            "Every dog has his day",
            "Every man has his price",
            "Every stick has two ends",
            "Everybody wants to go to heaven but nobody wants to die",
            "Everybody's business is nobody's business",
            "Fish and guests smell after three days",
            "Flattery will get you nowhere",
            "Fools rush in where angels fear to tread",
            "Forewarned is forearmed",
            "Fortune favours the brave"
        }

        'Shift + Alt + Symbol
        Dim s1 As New KEY
        s1.using_shift = True
        s1.using_ctrl = False
        s1.using_alt = True
        s1.symbol = "A"
        s1.keycode = 65
        s1.layout_needed = "Британская|США"
        s1.name = ""

        Dim s2 As New KEY
        s2.using_shift = True
        s2.using_ctrl = False
        s2.using_alt = True
        s2.symbol = "N"
        s2.keycode = 78
        s2.layout_needed = "Британская|США"
        s2.name = ""

        Dim s3 As New KEY
        s3.using_shift = True
        s3.using_ctrl = False
        s3.using_alt = True
        s3.symbol = "R"
        s3.keycode = 82
        s3.layout_needed = "Британская|США"
        s3.name = ""

        Dim s4 As New KEY
        s4.using_shift = True
        s4.using_ctrl = False
        s4.using_alt = True
        s4.symbol = "P"
        s4.keycode = 80
        s4.layout_needed = "Британская|США"
        s4.name = ""

        Dim s5 As New KEY
        s5.using_shift = True
        s5.using_ctrl = False
        s5.using_alt = True
        s5.symbol = "Z"
        s5.keycode = 90
        s5.layout_needed = "Британская|США"
        s5.name = ""

        'Shift + Ctrl + Symbol
        Dim s6 As New KEY
        s6.using_shift = True
        s6.using_ctrl = True
        s6.using_alt = False
        s6.symbol = "V"
        s6.keycode = 86
        s6.layout_needed = "Британская|США"
        s6.name = ""

        Dim s7 As New KEY
        s7.using_shift = True
        s7.using_ctrl = True
        s7.using_alt = False
        s7.symbol = "T"
        s7.keycode = 84
        s7.layout_needed = "Британская|США"
        s7.name = ""

        Dim s8 As New KEY
        s8.using_shift = True
        s8.using_ctrl = True
        s8.using_alt = False
        s8.symbol = "D"
        s8.keycode = 68
        s8.layout_needed = "Британская|США"
        s8.name = ""

        Dim s9 As New KEY
        s9.using_shift = True
        s9.using_ctrl = True
        s9.using_alt = False
        s9.symbol = "S"
        s9.keycode = 83
        s9.layout_needed = "Британская|США"
        s9.name = ""

        Dim s10 As New KEY
        s10.using_shift = True
        s10.using_ctrl = True
        s10.using_alt = False
        s10.symbol = "A"
        s10.keycode = 65
        s10.layout_needed = "Британская|США"
        s10.name = ""

        'Ctrl + Alt + Symbol
        Dim s21 As New KEY
        s21.using_shift = False
        s21.using_ctrl = True
        s21.using_alt = True
        s21.symbol = "D"
        s21.keycode = 68
        s21.layout_needed = "Британская|США"
        s21.name = ""

        Dim s22 As New KEY
        s22.using_shift = False
        s22.using_ctrl = True
        s22.using_alt = True
        s22.symbol = "W"
        s22.keycode = 87
        s22.layout_needed = "Британская|США"
        s22.name = ""

        Dim s23 As New KEY
        s23.using_shift = False
        s23.using_ctrl = True
        s23.using_alt = True
        s23.symbol = "C"
        s23.keycode = 67
        s23.layout_needed = "Британская|США"
        s23.name = ""

        Dim s24 As New KEY
        s24.using_shift = False
        s24.using_ctrl = True
        s24.using_alt = True
        s24.symbol = "L"
        s24.keycode = 76
        s24.layout_needed = "Британская|США"
        s24.name = ""

        Dim s25 As New KEY
        s25.using_shift = False
        s25.using_ctrl = True
        s25.using_alt = True
        s25.symbol = "Z"
        s25.keycode = 90
        s25.layout_needed = "Британская|США"
        s25.name = ""

        KEY_SHORTCUTS = {s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s21, s22, s23, s24, s25}
    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click
        End
    End Sub

    Private Sub Button6_Click(sender As System.Object, e As System.EventArgs) Handles Button6.Click
        Me.Panel9.Visible = False
        Me.Panel1.Visible = True
    End Sub

    Private Sub Button5_Click(sender As System.Object, e As System.EventArgs) Handles Button5.Click
        SUMMARY.ShowDialog()
    End Sub
End Class
