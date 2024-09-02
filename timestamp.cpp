#include "ui_timestamp.h"
#include <timestamp.h>
#include <QLineEdit>
#include <QDateTime>
#include <QTimer>

timestamp::timestamp(QWidget *parent)
    : QWidget(parent)
    , ui(new Ui::timestamp)
{
    ui->setupUi(this);

    QLineEdit *test = this->findChild<QLineEdit*>("nowLineEdit");

    // Set initial timestamp
    if (test) {
        test->setText(QDateTime::currentDateTime().toString("yyyy-MM-dd HH:mm:ss"));

        // Create a timer to update the timestamp every second
        QTimer *timer = new QTimer(this);
        connect(timer, &QTimer::timeout, [test]() {
            test->setText(QString::number(QDateTime::currentSecsSinceEpoch()));
        });
        timer->start(1000);  // Update every second
    }
}

timestamp::~timestamp()
{
    delete ui;
}

void timestamp::on_startButton_clicked()
{

}

