#ifndef TIMESTAMP_H
#define TIMESTAMP_H

#include <QWidget>

namespace Ui {
class Timestamp;
}

class Timestamp : public QWidget
{
    Q_OBJECT


public:
    explicit Timestamp(QWidget *parent = nullptr);
    ~Timestamp();

private slots:
    void on_startButton_clicked();

    void on_endButton_clicked();

    void on_timestampConvertButton_clicked();

    void on_datetimeConvertButton_clicked();

private:
    Ui::Timestamp *ui;

    bool isStop = false;

protected:
    void resizeEvent(QResizeEvent *event) override;
};

#endif // TIMESTAMP_H
