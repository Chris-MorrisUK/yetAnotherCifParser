   M S B A M L   `   `   `  ����  ScheduleVis    ScheduleVis.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
MainWindow��5   	   $	��350��6   $
��1024��6)    � WindowState$ 	Maximized=�66   .��5      � .+�5   
   �� -   5         ��Name$ btnLoad��5   \   $	��120��6   $��30��6   $
��Left=�6)   $	��Top=�6D   .��6�   	Load Data6�   �� 5   
   $��Visible=�6   $��0,40,0,0q�6>   .��5   
   �� -   5         ��Name$ cnvServices��5      $��Stretch=�6&   $��Stretch=�6D   
�������5	      5
      �� 5   
    �� Orientation$ 
Horizontal=�6   $��	130,0,0,0q�6.   $��40��6A   $	��Top=�6M   .+�5      �� -   5        $��btnImportStations��5   [   $��Import Station Names��6   $��30��64   $
��Left=�6@   $	��Top=�6t   $	��140��6�   $��5q�6�   �� 5      -   5        $��btnImportSchedules��5   W   $��Import Schedules��6   $��30��60   $
��Left=�6<   $	��Top=�6q   $	��140��6�   $��5q�6�   �� 5      -   5        $��btnCombinedImport��5   V   $��Combined Import��6   $��30��6/   $
��Left=�6;   $	��Top=�6o   $	��140��6�   $��5q�6�   �� 5      -   5        $��button1��5   6   $��
Load graph��6   $��30��6*   $	��140��6E   6Q   �� 5      -   5        $��tbNameLabel��5   ?   $��23��6   $
��Left=�6$   $��Center=�6R   $	��172��6m   $��5q�6y   Person responsible for update6�   �� 5      -   5        $��txtName��5   =   $��23��6   $
��Left=�6"   $��Center=�6L   $	��172��6g   $��5q�6s   	� 5      -	   5        $��rbUri��5   ]   $	��Uri��6   $��16��6(   $
��Left=�64   $	��5,7q�6P   $��Center=�6j    	� 	GroupName$ NameType��6�   	� 5      -
   5        $��rbLit��5   `   $��Literal��6   $��16��6,   $
��Left=�68   $	��5,7q�6S   $��Center=�6m    d� 	IsChecked$
 TrueY�6�   $ NameType��6�   �� 5      -   5         ��Name$ chkIncludeProv��5      $��Include Prov��6/   $	��5,7q�6G   $��Center=�6T   $
��Left=�6o   5      �� 5   
   -   5         ��Name$ txtStatusMessages��5      $��Center=�6-   $��Center=�6J   	 �� TextAlignment$	 Center=�6e   $	��400��6|   $��30��6�   $��	0,-50,0,0q�6�   
������ $y�Hidden=�6�   � 5   
   -   5        
 �Name$
 progress��5   h   $��Center=�6   $��30��63   $��Center=�6@   $	��400��6[    � Maximum$	 255Q�6z   $y�Hidden=�6�   5      5      