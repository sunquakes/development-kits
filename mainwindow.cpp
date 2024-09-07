#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <url.h>
#include <md5.h>
#include <base64.h>
#include <timestamp.h>
#include <JSON.h>

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    // Md5
    Md5 *newMD5Widget = new Md5();
    ui->tabWidget->insertTab(0, newMD5Widget, "Md5");
    ui->tabWidget->setCurrentIndex(0);
    // Url
    Url *newUrlWidge = new Url();
    ui->tabWidget->insertTab(1, newUrlWidge, "Url Encode/Decode");
    // Base64
    Base64 *newBase64Widge = new Base64();
    ui->tabWidget->insertTab(2, newBase64Widge, "Base64 Encode/Decode");

    // Timestamp
    Timestamp *newTimestamp = new Timestamp();
    ui->tabWidget->insertTab(3, newTimestamp, "Timestamp");

    // JSON
    JSON *newJSON = new JSON();
    ui->tabWidget->insertTab(4, newJSON, "JSON Format");
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::on_tabWidget_currentChanged(int index)
{
    QString formattedString = QString("Name: %1, Age: %2")
                                  .arg("")
                                  .arg(index);
    qInfo() << formattedString;
}


void MainWindow::on_tabWidget_tabBarDoubleClicked(int index)
{
    qInfo("b");
}


void MainWindow::on_tabWidget_tabBarClicked(int index)
{
    qInfo("c");
}

