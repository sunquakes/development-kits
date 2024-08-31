#include "url.h"
#include "ui_url.h"
#include <QUrl>
#include <QString>

QString urlEncode(const QString &url) {
    return QString::fromUtf8(QUrl::toPercentEncoding(url, QByteArray(), QByteArray("/")));
}

url::url(QWidget *parent)
    : QWidget(parent)
    , ui(new Ui::url)
{
    ui->setupUi(this);
}

url::~url()
{
    delete ui;
}

void url::on_encodeButton_clicked()
{
    QString text = ui->textEdit->toPlainText();
    if (!text.isEmpty()) {
        ui->textEdit->setText(urlEncode(text));
    }
}


void url::on_decodeButton_clicked()
{
    QString text = ui->textEdit->toPlainText();
    if (!text.isEmpty()) {
        qInfo() << text;
        QString decodedUrl = QUrl::fromPercentEncoding(text.toUtf8());
        qInfo() << decodedUrl;
        ui->textEdit->setText(decodedUrl);
    }
}


void url::on_clearButton_clicked()
{
    QString text = ui->textEdit->toPlainText();
    if (!text.isEmpty()) {
        ui->textEdit->setText("");
    }
}

