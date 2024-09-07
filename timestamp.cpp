#include "ui_timestamp.h"
#include <timestamp.h>
#include <QLineEdit>
#include <QDateTime>
#include <QTimer>

Timestamp::Timestamp(QWidget *parent)
    : QWidget(parent)
    , ui(new Ui::Timestamp)
{
    ui->setupUi(this);

    QLineEdit *nowLineEdit = this->findChild<QLineEdit*>("nowLineEdit");

    if (nowLineEdit) {
        nowLineEdit->setText(QDateTime::currentDateTime().toString("yyyy-MM-dd HH:mm:ss"));

        QTimer *timer = new QTimer(this);
        connect(timer, &QTimer::timeout, [nowLineEdit, this]() {
            if (!isStop) {
                nowLineEdit->setText(QString::number(QDateTime::currentSecsSinceEpoch()));
            }
        });
        timer->start(1000);  // Update every second
    }
}

Timestamp::~Timestamp()
{
    delete ui;
}

void Timestamp::on_startButton_clicked()
{
    isStop = false;
}


void Timestamp::on_endButton_clicked()
{
    isStop = true;
}


void Timestamp::on_timestampConvertButton_clicked()
{
    ui->timestampLineEdit_2->setText("");
    QString text = ui->timestampLineEdit->text();
    if (!text.isEmpty()) {
        qint64 timestamp = text.toLongLong();
        QDateTime dateTime;
        QString dateString;
        if (ui->timestampComboBox->currentText() == "Second") {
            dateTime = QDateTime::fromSecsSinceEpoch(timestamp);
            dateString = dateTime.toString("yyyy-MM-dd HH:mm:ss");
        } else {
            dateTime = QDateTime::fromMSecsSinceEpoch(timestamp);
            dateString = dateTime.toString("yyyy-MM-dd HH:mm:ss.zzz");
        }
        ui->timestampLineEdit_2->setText(dateString);
    }
}


void Timestamp::on_datetimeConvertButton_clicked()
{
    ui->datetimeLineEdit_2->setText("");
    QString text = ui->datetimeLineEdit->text();
    qint64 timestamp;
    if (!text.isEmpty()) {
        QDateTime dateTime = QDateTime::fromString(text, "yyyy-MM-dd HH:mm:ss");
        if (dateTime.isValid()) {
            if (ui->datetimeComboBox->currentText() == "Second") {
                timestamp = dateTime.toSecsSinceEpoch();
            } else {
                timestamp = dateTime.toMSecsSinceEpoch();
            }
            ui->datetimeLineEdit_2->setText(QString::number(timestamp));
        } else {
            QDateTime dateTime = QDateTime::fromString(text, "yyyy-MM-dd HH:mm:ss.zzz");
            if (dateTime.isValid()) {
                if (ui->datetimeComboBox->currentText() == "Second") {
                timestamp = dateTime.toSecsSinceEpoch();
                } else {
                timestamp = dateTime.toMSecsSinceEpoch();
                }
                ui->datetimeLineEdit_2->setText(QString::number(timestamp));
            }
        }
    }
}

void Timestamp::resizeEvent(QResizeEvent *event) {
    QWidget::resizeEvent(event);

    if (ui->widget) {
        int x = (width() - ui->widget->width()) / 2;
        int y = (height() - ui->widget->height()) / 2;
        ui->widget->move(x, y);
    }
}
