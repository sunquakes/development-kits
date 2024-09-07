#ifndef URL_H
#define URL_H

#include <QWidget>

namespace Ui {
class Url;
}

class Url : public QWidget
{
    Q_OBJECT

public:
    explicit Url(QWidget *parent = nullptr);
    ~Url();

private slots:
    void on_encodeButton_clicked();

    void on_decodeButton_clicked();

    void on_clearButton_clicked();

private:
    Ui::Url *ui;
};

#endif // URL_H
