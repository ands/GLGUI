GLGUI
=====

![GLGUI Example](http://andsz.de/i/GLGUI.png)

This OpenGL User Interface library is a WinForms-like object oriented GUI based on a custom OpenTK version which allows custom cursors (https://github.com/ands/opentk) and some QuickFont font generator code.
Both GameWindows and GLControls, which use different input EventArgs, are supported.

You can build the GLGUI assembly with or without System.Windows.Forms (=> with or without cursor support) by using the REFERENCE_WINDOWS_FORMS build flag.

You can build the GLGUI assembly with or without OpenTK.GLControl (=> with or without OpenTK WinForms context support) by using the REFERENCE_OPENTK_GLCONTROL build flag.
REFERENCE_OPENTK_GLCONTROL requires REFERENCE_WINDOWS_FORMS to be set.


TODO
====

  - Clean up (example) code
  - Add documentation
  - Get rid of last few legacy OpenGL dependencies
