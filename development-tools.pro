QT       += core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

CONFIG += c++17

# You can make your code fail to compile if it uses deprecated APIs.
# In order to do so, uncomment the following line.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

SOURCES += \
    base64.cpp \
    json.cpp \
    main.cpp \
    mainwindow.cpp \
    md5.cpp \
    timestamp.cpp \
    url.cpp

HEADERS += \
    base64.h \
    json.h \
    mainwindow.h \
    md5.h \
    timestamp.h \
    url.h

FORMS += \
    base64.ui \
    json.ui \
    mainwindow.ui \
    md5.ui \
    timestamp.ui \
    url.ui

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target

TRANSLATIONS += translations/i18n_zh.ts \
                 translations/i18n_en.ts
