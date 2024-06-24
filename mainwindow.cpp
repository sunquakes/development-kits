#include "mainwindow.h"
#include "ui_mainwindow.h"

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);
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

