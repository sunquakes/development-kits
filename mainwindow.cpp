#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <md5.h>

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    md5 *newMD5Widget = new md5();
    ui->tabWidget->insertTab(0, newMD5Widget, "New Tab");
    ui->tabWidget->setCurrentIndex(0);
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
