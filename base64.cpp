#include "base64.h"
#include "ui_base64.h"

Base64::Base64(QWidget *parent)
    : QWidget(parent)
    , ui(new Ui::Base64)
{
    ui->setupUi(this);
}

Base64::~Base64()
{
    delete ui;
}

void Base64::on_encodeButton_clicked()
{
    QString text = ui->textEdit->toPlainText();
    if (!text.isEmpty()) {
        QByteArray byteArray = text.toUtf8();
        QByteArray base64Encoded = byteArray.toBase64();
        QString encodedString = QString::fromUtf8(base64Encoded);
        ui->textEdit->setText(encodedString);
    }
}


void Base64::on_decodeButton_clicked()
{
    QString text = ui->textEdit->toPlainText();
    if (!text.isEmpty()) {
        qInfo() << text;
        QByteArray byteArray = text.toUtf8();
        QByteArray decodedByteArray = QByteArray::fromBase64(byteArray);

        QString decodedString = QString::fromUtf8(decodedByteArray);
        qInfo() << decodedString;
        ui->textEdit->setText(decodedString);
    }
}


void Base64::on_clearButton_clicked()
{
    QString text = ui->textEdit->toPlainText();
    if (!text.isEmpty()) {
        ui->textEdit->setText("");
    }
}

