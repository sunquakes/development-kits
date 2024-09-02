#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <url.h>
#include <md5.h>
#include <base64.h>
#include <timestamp.h>

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    // md5
    md5 *newMD5Widget = new md5();
    ui->tabWidget->insertTab(0, newMD5Widget, "md5");
    ui->tabWidget->setCurrentIndex(0);
    // url
    url *newUrlWidge = new url();
    ui->tabWidget->insertTab(1, newUrlWidge, "url encode/decode");
    // base64
    base64 *newBase64Widge = new base64();
    ui->tabWidget->insertTab(2, newBase64Widge, "base64 encode/decode");

    // timestamp
    timestamp *newTimestamp = new timestamp();
    ui->tabWidget->insertTab(3, newTimestamp, "Timestamp");
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

