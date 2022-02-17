﻿Public Class layoutForm
    Shared allArea, wide, wid(3), wi(18), len(18), h(4), v(18), x(18, 2), y(18, 2), area(18), myWidth, myHeight As Single
    Dim ratioX, ratioY, tmp As Single : Dim calib As Integer = 30 : Dim marg_l As Integer = 150 : Dim marg_b As Integer = 150
    Dim center(18), scorePoint As Point : Dim dept(18) As String
    Dim bay1(6), bay2(6), bay3(6), c1, c2, c3, depNo, changeCount, max, idx, depIdx, index, score(18, 18) As Integer
    Dim lblCtx(18), lblCty(18), lblLength(18), lblWidth(18), dep(18), lblL_W(18), lblArea(18) As Label
    Dim lblLbound, lblUbound, lblRscore, colScore(18), rowScore(18) As Label
    Dim ctx(18), cty(18), L_W(18), lBound, uBound, rScore, relDist As Single
    Dim boldFont As New Font("Arial", 9, FontStyle.Bold)
    Dim myPen, boxPen As Pen
    Dim WithEvents genBut, changeBut, autoBut, saveBut, openBut As Button
    Dim myRect As New Rectangle
    Dim lblAtVer, lblAtHor, title1, title2, txtLbound, txtUbound, txtRscore, lblFile, lblFileName As Label
    Dim txtAtVer, txtAtHor, txtTest As TextBox
    Dim relList(), distList() As Single
    Dim pairIdx, maxPair As Integer : Dim curPair, pairElm(2), chgTemp As String
    Dim ind() As Integer : Dim pairChg As New Hashtable
    Dim saveLayoutDialog As New SaveFileDialog : Dim openLayoutDialog As New OpenFileDialog
    Dim department(), areawide(), vector(), bay(), deptIndex(), ratio() As String



    Private Sub layoutForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        depIdx = 0
        'TESTING
        title1 = New Label() With {.Location = New Point(10, 15), .Width = 300}
        title2 = New Label() With {.Location = New Point(10, 40), .Width = 400}
        title1.Text = Space(20) & "CENTROIDS"
        title2.Text = "DEPT" & Space(10) & "X" & Space(15) & "Y" & Space(10) & "LENGHT" & Space(10) _
                       & "WIDTH" & Space(10) & "L/W" & Space(10) & "AREA"
        title1.Font = boldFont : title2.Font = boldFont
        Me.Controls.Add(title1) : Me.Controls.Add(title2)
        depNo = scoreForm.count
        lblLbound = New Label() With {.Location = New Point(10, title2.Location.Y + 25 * depNo + 25), .Width = 60, .Text = "L.Bound : "}
        txtLbound = New Label() With {.Location = New Point(70, lblLbound.Location.Y), .Width = 50, .Font = New Font(Me.Font, FontStyle.Bold)}
        lblUbound = New Label() With {.Location = New Point(10, lblLbound.Location.Y + 25), .Width = 60, .Text = "U.Bound : "}
        txtUbound = New Label() With {.Location = New Point(70, lblUbound.Location.Y), .Width = 50, .Font = New Font(Me.Font, FontStyle.Bold)}
        lblRscore = New Label() With {.Location = New Point(10, lblUbound.Location.Y + 25), .Width = 60, .Text = "R.Score : "}
        txtRscore = New Label() With {.Location = New Point(70, lblRscore.Location.Y), .Width = 50, .Font = New Font(Me.Font, FontStyle.Bold)}
        Me.Controls.Add(lblLbound) : Me.Controls.Add(txtLbound)
        Me.Controls.Add(lblUbound) : Me.Controls.Add(txtUbound)
        Me.Controls.Add(lblRscore) : Me.Controls.Add(txtRscore)
        scorePoint = New Point(txtLbound.Location.X + 70, txtLbound.Location.Y)
        For i = 1 To depNo
            dept(i) = formDept.txtDept(i).Text
            area(i) = CSng(formDept.txtArea(i).Text)
        Next
        Dim therand As New Random
        Dim theCount As Integer = 0
        For i = 1 To depNo - 1
            For j = 2 To depNo
                If i >= j Then
                    'nothing
                Else
                    score(i, j) = scoreForm.scoreVal(i, j)
                    'Variables to save scores
                    theCount += 1
                    ReDim Preserve vector(theCount)
                    vector(theCount) = score(i, j)
                End If
            Next
        Next
        myPen = New Pen(Drawing.Color.Blue, 3) : boxPen = New Pen(Drawing.Color.Red, 2)
        Dim allArea As Single = 0
        For k = 1 To depNo
            allArea += area(k)
        Next
        calib = 540 / Math.Sqrt(allArea)
        change_pair()
        wide = allArea
        myRect.X = marg_l
        myRect.Y = 10
        myWidth = Math.Sqrt((CSng(ratioForm.ratioX) / CSng(ratioForm.ratioY)) * allArea)
        myHeight = Math.Sqrt((CSng(ratioForm.ratioY) / CSng(ratioForm.ratioX)) * allArea)
        myRect.Width = myWidth * calib
        myRect.Height = myHeight * calib
        Me.Size = New Size(myRect.Width + marg_l + 280, myRect.Height + marg_b + 100)
        Me.CenterToScreen()
        'Create Button to randomize layout
        genBut = New Button() With {.Location = New Point(Me.Width - 120, myRect.Height + 60), .Text = "Random Layout", .Width = 90, .Height = 30}
        AddHandler genBut.Click, AddressOf generate_layout
        changeBut = New Button() With {.Location = New Point(Me.Width - 220, myRect.Height + 60), .Text = "Change Dept", .Width = 90, .Height = 30}
        AddHandler changeBut.Click, AddressOf change_dept
        autoBut = New Button() With {.Location = New Point(Me.Width - 340, myRect.Height + 60), .Text = "Automatic Search", .Width = 110, .Height = 30}
        AddHandler autoBut.Click, AddressOf auto_search
        saveBut = New Button() With {.Location = New Point(Me.Width - 450, myRect.Height + 60), .Text = "Save Layout", .Width = 70, .Height = 40}
        AddHandler saveBut.Click, AddressOf save_layout
        openBut = New Button() With {.Location = New Point(Me.Width - 530, myRect.Height + 60), .Text = "Open Layout", .Width = 70, .Height = 40}
        AddHandler openBut.Click, AddressOf open_layout
        Me.Controls.Add(genBut) : Me.Controls.Add(changeBut) : Me.Controls.Add(autoBut) : Me.Controls.Add(saveBut) : Me.Controls.Add(openBut)
        lblFile = New Label() With {.Location = New Point(Me.Width - myRect.Width - 30, myRect.Y - 5), .Text = "File : ", .Width = 30, .Height = 40}
        lblFileName = New Label() With {.Location = New Point(Me.Width - myRect.Width + 5, myRect.Y - 5), .Text = "New", .Font = New Font(Me.Font, FontStyle.Bold), _
                                    .Width = 300, .Height = 40}
        Me.Controls.Add(lblFile) : Me.Controls.Add(lblFileName)
        layoutPanel.Size = New Size(myRect.Width + 10, myRect.Height + 10)
        layoutPanel.Location = New Point(Me.Width - myRect.Width - 30, 30)
        layoutPanel.BackColor = Color.Aqua
        random_layout()
        ReDim Preserve ind(depNo)
        Dim allNumbers As New List(Of Integer)
        Dim randomNumbers As New List(Of Integer)
        Dim rand As New Random

        ' Fill the list of all numbers
        For i As Integer = 1 To depNo
            allNumbers.Add(i)
        Next i

        ' Grab a random entry from the list of all numbers
        For i As Integer = 1 To depNo
            Dim selectedIndex As Integer = rand.Next(0, (allNumbers.Count - 1))
            Dim selectedNumber As Integer = allNumbers(selectedIndex)
            randomNumbers.Add(selectedNumber)
            allNumbers.Remove(selectedNumber)
            ind(i) = selectedNumber
            'Variables to save index of departments
            ReDim Preserve deptIndex(i)
            deptIndex(i) = ind(i)
        Next i
        For i = 1 To depNo
            lblCtx(i) = New Label() With {.Location = New Point(60, 40 + i * 25), .Width = 30}
            lblCty(i) = New Label() With {.Location = New Point(120, 40 + i * 25), .Width = 30}
            dep(i) = New Label() With {.Location = New Point(20, 40 + i * 25), .Width = 30}
            lblWidth(i) = New Label() With {.Location = New Point(170, 40 + i * 25), .Width = 30}
            lblLength(i) = New Label() With {.Location = New Point(240, 40 + i * 25), .Width = 30}
            lblL_W(i) = New Label() With {.Location = New Point(300, 40 + i * 25), .Width = 30}
            lblArea(i) = New Label() With {.Location = New Point(360, 40 + i * 25), .Width = 30}
            Me.Controls.Add(lblCtx(i))
            Me.Controls.Add(lblCty(i))
            Me.Controls.Add(dep(i))
            Me.Controls.Add(lblWidth(i))
            Me.Controls.Add(lblLength(i))
            Me.Controls.Add(lblL_W(i))
            Me.Controls.Add(lblArea(i))
            'Variables to save Departments and areas
            ReDim Preserve department(i) : ReDim Preserve areawide(i)
            department(i) = dept(i)
            areawide(i) = area(i)
        Next
    End Sub

    Private Sub layoutPanel_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles layoutPanel.Paint
        drawLayout(e)
        'attachedTest()
    End Sub
    Sub drawLayout(e)
        count_layout()
        e.Graphics.FillRectangle(Brushes.White, 0, 0, myRect.Width, myRect.Height)
        'Dept Name
        For i = 1 To depNo
            center(i) = New Point(x(i, 1) + (x(i, 2) - x(i, 1)) / 2, y(i, 1) + (y(i, 2) - y(i, 1)) / 2)
            e.Graphics.DrawString(dept(ind(i)).Replace(Chr(32), vbLf), Font, Brushes.Black, center(i))
        Next
        txtLbound.Text = lBound : txtUbound.Text = uBound : txtRscore.Text = rScore
        'rectangle
        e.Graphics.DrawRectangle(myPen, myRect)
        For n = 1 To depNo
            e.Graphics.DrawRectangle(boxPen, x(n, 1), y(n, 1), x(n, 2) - x(n, 1), y(n, 2) - y(n, 1))
        Next
    End Sub
    Sub count_layout()
        Me.Size = New Size(myRect.Width + marg_l + 280, myRect.Height + marg_b + 100)
        myRect.X = 0
        myRect.Y = 0
        wide = 0
        For k = 1 To depNo
            wide += area(ind(k))
            'MessageBox.Show(area(k))
        Next
        calib = 540 / Math.Sqrt(wide)
        myWidth = Math.Sqrt((CSng(ratioForm.ratioX) / CSng(ratioForm.ratioY)) * wide)
        myHeight = Math.Sqrt((CSng(ratioForm.ratioY) / CSng(ratioForm.ratioX)) * wide)
        myRect.Width = myWidth * calib
        myRect.Height = myHeight * calib
        layoutPanel.Size = New Size(myRect.Width + 10, myRect.Height + 10)
        layoutPanel.Location = New Point(Me.Width - myRect.Width - 30, 30)
        layoutPanel.BackColor = Color.Aqua
        genBut.Location = New Point(Me.Width - 120, myRect.Height + 60)
        changeBut.Location = New Point(Me.Width - 220, myRect.Height + 60)
        autoBut.Location = New Point(Me.Width - 340, myRect.Height + 60)
        saveBut.Location = New Point(Me.Width - 450, myRect.Height + 60)
        openBut.Location = New Point(Me.Width - 530, myRect.Height + 60)
        index = 1
        'loop to count area wide
        wid(1) = 0 : wid(2) = 0 : wid(3) = 0
        For k = 1 To c1
            wid(1) += area(ind(index))
            index += 1
        Next
        For l = 1 To c2
            wid(2) += area(ind(index))
            index += 1
        Next
        For m = 1 To c3
            wid(3) += area(ind(index))
            index += 1
        Next
        'loop to count area lenght
        wid(1) = Math.Round(wid(1) / myWidth, 3) : wid(2) = Math.Round(wid(2) / myWidth, 3) : wid(3) = Math.Round(wid(3) / myWidth, 3)
        index = 1
        For k = 1 To c1
            len(ind(index)) = area(ind(index)) / wid(1)
            wi(ind(index)) = wid(1)
            index += 1
        Next
        For l = 1 To c2
            len(ind(index)) = area(ind(index)) / wid(2)
            wi(ind(index)) = wid(2)
            index += 1
        Next
        For m = 1 To c3
            len(ind(index)) = area(ind(index)) / wid(3)
            wi(ind(index)) = wid(3)
            index += 1
        Next
        'loop to count x1,y1 and x2,y2 of area position
        x(1, 1) = 0 : y(1, 1) = 0
        index = 1
        For k = 1 To c1
            If k = 1 Then
                x(index, 1) = x(1, 1)
                y(index, 1) = y(1, 1)
            Else
                x(index, 1) = x(index - 1, 2)
                y(index, 1) = y(index - 1, 1)
            End If
            x(index, 2) = x(index, 1) + len(ind(index)) * calib
            y(index, 2) = y(index, 1) + wid(1) * calib
            index += 1
        Next
        For l = 1 To c2
            If l = 1 Then
                x(index, 1) = x(1, 1)
                y(index, 1) = y(1, 1) + wid(1) * calib
            Else
                x(index, 1) = x(index - 1, 2)
                y(index, 1) = y(1, 1) + wid(1) * calib
            End If
            x(index, 2) = x(index, 1) + len(ind(index)) * calib
            y(index, 2) = y(index, 1) + wid(2) * calib
            index += 1
        Next
        For m = 1 To c3
            If m = 1 Then
                x(index, 1) = x(1, 1)
                y(index, 1) = y(1, 1) + (wid(1) + wid(2)) * calib
            Else
                x(index, 1) = x(index - 1, 2)
                y(index, 1) = y(1, 1) + (wid(1) + wid(2)) * calib
            End If
            x(index, 2) = x(index, 1) + len(ind(index)) * calib
            y(index, 2) = y(index, 1) + wid(3) * calib
            index += 1
        Next
        'Dept Name
        Dim font As Font = Me.Font
        For i = 1 To depNo
            center(i) = New Point(x(i, 1) + (x(i, 2) - x(i, 1)) / 2, y(i, 1) + (y(i, 2) - y(i, 1)) / 2)
            ctx(i) = center(i).X / calib
            cty(i) = center(i).Y / calib
            L_W(i) = CSng(len(ind(i)) / wi(ind(i)))
            lblCtx(i).Text = ctx(i)
            lblCty(i).Text = cty(i)
            dep(i).Text = dept(ind(i))
            lblWidth(i).Text = len(ind(i))
            lblLength(i).Text = wi(ind(i))
            lblL_W(i).Text = L_W(i)
            lblArea(i).Text = area(ind(i))
        Next
        ''Search for relDis elements count
        Dim count As Integer = 0
        For n = 1 To depNo - 1
            For m = 2 To depNo
                If n >= m Then
                    'nothing
                Else
                    count += 1
                End If
            Next
        Next
        ReDim distList(count - 1) : ReDim relList(count - 1)
        'Return
        'Counting Rel-Dist Value
        'txtTest.Text = ""
        idx = 0
        For i = 1 To depNo - 1
            For j = 2 To depNo
                If i >= j Then
                    'nothing
                Else
                    'MessageBox.Show(idx)
                    relList(idx) = score(i, j)
                    distList(idx) = CSng(((Math.Abs(ctx(i) - ctx(j)) + Math.Abs(cty(i) - cty(j)).ToString)))
                    'txtTest.Text &= "(" & i & "," & j & " - " & relList(idx) & "," & Math.Round(distList(idx)) & " - " & Math.Round(ctx(i), 1) & " , " & Math.Round(ctx(j), 1) & ")"
                    'txtTest.Text &= "== " & idx & " == (" & relList(idx) & "," & Math.Round(distList(idx)) & ")"
                    idx += 1
                End If
            Next
        Next
        uBound = 0 : lBound = 0 : relDist = 0
        'txtAtHor.Text = "" : txtAtVer.Text = ""
        Dim cek As String = ""
        For i = 0 To idx - 1
            relDist += relList(i) * distList(i)
            'cek &= relList(i) & " x " & distList(i) & "  +  "
            cek &= distList(i) & " - "
        Next
        Array.Sort(distList) : Array.Sort(relList)
        Array.Reverse(relList)
        cek = ""
        For i = 0 To idx - 1
            lBound += relList(i) * distList(i)
            'txtAtVer.Text += "(" & relList(i) & " x " & distList(i) & ") "
            cek &= distList(i) & " - "
        Next
        Array.Reverse(distList)
        For i = 0 To idx - 1
            uBound += relList(i) * distList(i)
            'txtAtHor.Text += "(" & relList(i) & " x " & distList(i) & ") "
        Next
        rScore = 1 - (relDist - lBound) / (uBound - lBound)
    End Sub
    Sub random_layout()
        Dim rnd As New Random
        If depNo > 12 Then
            c1 = rnd.Next(depNo - 12, 6)
            If (c1 < 6) Then c2 = 6 Else c2 = rnd.Next(depNo - 12, 6)
            c3 = depNo - c1 - c2
        ElseIf depNo > 6 And depNo <= 12 Then
            c1 = rnd.Next(1, 6)
            If ((depNo - c1) >= 6) Then c2 = rnd.Next(1, 6) Else c2 = rnd.Next(1, depNo - c1)
            c3 = depNo - c1 - c2
        ElseIf depNo <= 6 Then
            c1 = rnd.Next(1, depNo)
            If (c1 = depNo) Then c2 = 0 Else c2 = rnd.Next(1, depNo - c1)
            If ((depNo - c1 - c2) < 0) Then c3 = 0 Else c3 = depNo - c1 - c2
        End If
        'Variables to save bays/rows
        ReDim Preserve bay(3)
        bay(1) = c1
        bay(2) = c2
        bay(3) = c3
    End Sub
    Sub generate_layout(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles genBut.Click
        change_pair()
        random_layout()
        change_randomDeptId()
        layoutPanel.Invalidate()
    End Sub
    'Horizontal closing objects analysis
    Function attached_horizontal(left1, top1, right1, bottom1, left2, top2, right2, bottom2)
        attached_horizontal = (right1 = left2 Or left1 = right2) And _
    ((top1 <= bottom2 And top1 >= top2) Or (bottom1 <= bottom2 And bottom1 >= top2))
    End Function

    'Vertical closing objects analysis
    Function attached_vertical(left1, top1, right1, bottom1, left2, top2, right2, bottom2)
        attached_vertical = (bottom1 = top2 Or top1 = bottom2) And _
        ((right1 >= left2 And right1 <= right2) Or (left1 >= left2 And left1 <= right2) Or (left1 <= left2 And right1 >= right2))
    End Function

    Private Sub attachedTest()
        'MessageBox.Show(ind(3))
        'txtAtHor.Text = "" : txtAtVer.Text = ""
        For p = 1 To depNo - 1
            For q = p + 1 To depNo
                If (attached_horizontal(Math.Ceiling(x(p, 1)), Math.Ceiling(y(p, 1)), Math.Ceiling(x(p, 2)), Math.Ceiling(y(p, 2)), Math.Ceiling(x(q, 1)), Math.Ceiling(y(q, 1)), Math.Ceiling(x(q, 2)), Math.Ceiling(y(q, 2)))) Then
                    'txtAtHor.Text += "(" & ind(p) & "-" & ind(q) & ") "
                End If
            Next
        Next
        For p = 1 To depNo - 1
            For q = p + 1 To depNo
                'txtTest.Text += "(" & p & "-" & q & ") "
                If (attached_vertical(Math.Ceiling(x(p, 1)), Math.Ceiling(y(p, 1)), Math.Ceiling(x(p, 2)), Math.Ceiling(y(p, 2)), Math.Ceiling(x(q, 1)), Math.Ceiling(y(q, 1)), Math.Ceiling(x(q, 2)), Math.Ceiling(y(q, 2)))) Then
                    'txtAtVer.Text += "(" & ind(p) & "-" & ind(q) & ") "
                End If
            Next
        Next
    End Sub
    Sub change_pair()
        pairChg.Clear()
        Dim indPair As Integer = 0
        For m = 1 To depNo - 1
            For n = 2 To depNo
                If m >= n Then
                    'nothing
                Else
                    indPair += 1
                    pairChg.Add(indPair, m & "-" & n)
                End If
            Next
        Next
        depIdx = 0
        maxPair = indPair
    End Sub
    Private Sub change_dept()
        change_deptId()
        layoutPanel.Invalidate()
    End Sub
    Sub change_deptId()
        If depIdx >= maxPair Then
            depIdx = 1
        Else
            depIdx += 1
        End If
        curPair = pairChg.Item(depIdx)
        pairElm = curPair.Split("-")
        Dim elm1 As Integer = Array.FindIndex(ind, Function(s) s = (pairElm(0)))
        Dim elm2 As Integer = Array.FindIndex(ind, Function(s) s = (pairElm(1)))
        chgTemp = ind(elm1)
        ind(elm1) = ind(elm2)
        ind(elm2) = chgTemp
    End Sub
    Sub change_randomDeptId()
        Dim rndId As New Random
        For i = 1 To depNo
            Dim elm1 As Integer = rndId.Next(1, depNo)
            Dim elm2 As Integer = rndId.Next(1, depNo)
            chgTemp = ind(elm1)
            ind(elm1) = ind(elm2)
            ind(elm2) = chgTemp
        Next
    End Sub

    Private Sub spSort(list As ListBox)
        Dim m As New Object
        list.Items.CopyTo(m, 0)
        Array.Sort(m)
        list.Items.AddRange(m)
    End Sub

    Private Sub auto_search()
        Dim maxScore As Single = 0
        Dim cScore, maxInd() As Integer
        cScore = 0 : ReDim maxInd(depNo)
        For iter = 1 To 500
            change_deptId()
            count_layout()
            If rScore > maxScore Then
                maxScore = rScore
                cScore += 1
                For i = 1 To depNo
                    maxInd(i) = ind(i)
                Next
            End If
        Next
        'MessageBox.Show(maxScore)
        For i = 1 To depNo
            ind(i) = maxInd(i)
        Next
        layoutPanel.Invalidate()
    End Sub
    Private Sub save_layout(sender As Object, e As EventArgs)
        Dim fileText As String = lblFileName.Text
        If fileText = "New" Then
            saveLayoutDialog.Filter = "TXT Files (*.txt*)|*.txt"
            If saveLayoutDialog.ShowDialog = Windows.Forms.DialogResult.OK _
            Then
                My.Computer.FileSystem.WriteAllText _
                (saveLayoutDialog.FileName, theTextToSave, False)
                lblFileName.Text = saveLayoutDialog.FileName
            End If
        End If
        If System.IO.File.Exists(fileText) = True Then
            Dim result As Integer = MessageBox.Show("Do you want to replace current file?", "Warning", MessageBoxButtons.YesNoCancel)
            If result = DialogResult.Cancel Then
                'nothing
            ElseIf result = DialogResult.No Then
                saveLayoutDialog.Filter = "TXT Files (*.txt*)|*.txt"
                If saveLayoutDialog.ShowDialog = Windows.Forms.DialogResult.OK _
                Then
                    My.Computer.FileSystem.WriteAllText _
                    (saveLayoutDialog.FileName, theTextToSave, False)
                End If
                lblFileName.Text = saveLayoutDialog.FileName
            ElseIf result = DialogResult.Yes Then
                My.Computer.FileSystem.WriteAllText _
                (fileText, theTextToSave, False)
            End If
        End If
    End Sub

    Private Sub open_layout(sender As Object, e As EventArgs)
        Dim dep, are, vec, row, dIn, rat As Array
        Dim strFileName() As String
        Dim tempStr As String = ""
        Dim arrl As Integer = 0 : Dim thecount As Integer = 0
        openLayoutDialog.Filter = "TXT Files (*.txt*)|*.txt"
        If openLayoutDialog.ShowDialog = DialogResult.OK Then
            lblFileName.Text = openLayoutDialog.FileName
            strFileName = IO.File.ReadAllLines(openLayoutDialog.FileName)
            For Each myLine In strFileName '// loop thru Arrays.
                tempStr = myLine
                If arrl = 0 Then
                    department = tempStr.Split(":")
                    dep = department(1).Split(",")
                    depNo = dep.Length
                    For i = 0 To depNo - 1
                        dept(i + 1) = dep(i)
                    Next
                ElseIf arrl = 1 Then
                    areawide = tempStr.Split(":")
                    are = areawide(1).Split(",")
                    For i = 0 To depNo - 1
                        area(i + 1) = are(i)
                    Next
                ElseIf arrl = 2 Then
                    vector = tempStr.Split(":")
                    vec = vector(1).Split(",")
                    thecount = 0
                    For i = 1 To depNo - 1
                        For j = 2 To depNo
                            If i >= j Then
                                'nothing
                            Else
                                thecount += 1
                            End If
                        Next
                    Next
                ElseIf arrl = 3 Then
                    bay = tempStr.Split(":")
                    row = bay(1).Split(",")
                    c1 = row(0)
                    c2 = row(1)
                    c3 = row(2)
                ElseIf arrl = 4 Then
                    deptIndex = tempStr.Split(":")
                    dIn = deptIndex(1).Split(",")
                    'MessageBox.Show(deptIndex(1))
                    'For i = 0 To depNo - 1
                    '    ind(i + 1) = dIn(i)
                    'Next
                ElseIf arrl = 5 Then
                    ratio = tempStr.Split(":")
                    rat = ratio(1).Split("/")
                    ratioForm.ratioX = rat(0)
                    ratioForm.ratioY = rat(1)
                End If
                arrl += 1
            Next
            ReDim Preserve ind(depNo)
            change_pair()
            count_layout()
            layoutPanel.Invalidate()
        End If
    End Sub

    Private Function theTextToSave() As String
        'department(), areawide(), vector(), bay(), deptIndex()
        Dim mydep, myare, myvec, mybay, myind As String
        mydep = arrToStr(department)
        myare = arrToStr(areawide)
        myvec = arrToStr(vector)
        mybay = arrToStr(bay)
        myind = arrToStr(deptIndex)
        Dim saveText As String
        saveText = "dept:" & mydep & vbLf & "area:" & myare & vbLf & "vector:" & myvec & vbLf & "bay:" & mybay & vbLf & "deptIndex:" & myind _
                   & vbLf & "ratio:" & ratioForm.ratioX & "/" & ratioForm.ratioY
        Return saveText
    End Function
    Private Function arrToStr(theArray As Array)
        Dim myStr As String : myStr = ""
        For i = 1 To theArray.Length - 1
            myStr &= theArray(i) & If(i >= theArray.Length - 1, "", ",")
        Next
        Return myStr
    End Function

End Class