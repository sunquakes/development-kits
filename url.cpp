#include "url.h"
#include "ui_url.h"
#include <QUrl>
#include <QString>

QString urlEncode(const QString &url) {
    return QString::fromUtf8(QUrl::toPercentEncoding(url, QByteArray(), QByteArray("/")));
}

Url::Url(QWidget *parent)
    : QWidget(parent)
    , ui(new Ui::Url)
{
    ui->setupUi(this);
}

Url::~Url()
{
    delete ui;
}

void Url::on_encodeButton_clicked()
{
    QString text = ui->textEdit->toPlainText();
    if (!text.isEmpty()) {
        ui->textEdit->setText(urlEncode(text));
    }
}


void Url::on_decodeButton_clicked()
{
    QString text = ui->textEdit->toPlainText();
    if (!text.isEmpty()) {
        qInfo() << text;
        QString decodedUrl = QUrl::fromPercentEncoding(text.toUtf8());
        qInfo() << decodedUrl;
        ui->textEdit->setText(decodedUrl);
    }
}


void Url::on_clearButton_clicked()
{
    QString text = ui->textEdit->toPlainText();
    if (!text.isEmpty()) {
        ui->textEdit->setText("");
    }
}

